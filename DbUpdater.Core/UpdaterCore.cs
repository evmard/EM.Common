using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Transactions;

namespace Crm.DbUpdater.Core
{
    public static class UpdaterCore
    {
        static PatchDbFactory  _dbFactory = new PatchDbFactory();
        public static bool CheckVersion(Version version, string nameOrConnectionString)
        {
            return version == GetDbVersion(nameOrConnectionString);
        }

        public static Version GetDbVersion(string nameOrConnectionString)
        {
            using (var db = _dbFactory.Create(nameOrConnectionString))
            {
                if (!db.Migrations.Any())
                    return new Version(0, 0, 0);

                return db.Migrations
                    .OrderByDescending(item => item.Major)
                    .OrderByDescending(item => item.Minor)
                    .OrderByDescending(item => item.Build)
                    .First()
                    .Version;
            }
        }

        public static PatchResult ApplyPatch(string pathFileName, string nameOrConnectionString)
        {
            if (!File.Exists(pathFileName))
                return new PatchResult { IsSuccess = false, Message = $"Файл не найден {pathFileName}" };

            var patchName = Path.GetFileNameWithoutExtension(pathFileName);
            var patchDir = $"{Path.GetTempPath()}{Guid.NewGuid().ToString()}";
            Directory.CreateDirectory(patchDir);
            ZipFile.ExtractToDirectory(pathFileName, patchDir);

            var infoPath = $"{patchDir}\\path_info.json";

            if (!File.Exists(infoPath))
                return new PatchResult { IsSuccess = false, Message = "Отсутсвует информация о патче" };

            var jsonStr = File.ReadAllText(infoPath, Encoding.UTF8);
            var patchInfo = JsonConvert.DeserializeObject<PatchInfo>(jsonStr);

            StringBuilder fileHashes = new StringBuilder();
            fileHashes.Append(patchInfo.From.ToString());
            foreach (var fileInfo in patchInfo.Files)
            {
                var filePath = $"{patchDir}\\{fileInfo.Name}";
                if (!File.Exists(filePath))
                    return new PatchResult { IsSuccess = false, Message = $"Отсутсвует файл {fileInfo.Name}" };

                var fileHash = GetFileHash(filePath);
                if (fileHash != fileInfo.Hash)
                    return new PatchResult { IsSuccess = false, Message = $"Нарушена целостность файла {fileInfo.Name}" };

                fileHashes.Append(fileHash);
            }
            fileHashes.Append(patchInfo.To.ToString());

            var patchHash = GetPatchHash(fileHashes);
            if (patchHash != patchInfo.Hash)
                return new PatchResult { IsSuccess = false, Message = "Нарушена целостность патча" };

            if (!CheckVersion(patchInfo.From, nameOrConnectionString))
                return new PatchResult { IsSuccess = false, Message = "Патч не может быть применен для данной версии базы" };

            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions
            {
                IsolationLevel = IsolationLevel.Serializable,
                Timeout = TimeSpan.FromMinutes(30)
            }))
            using(var db = _dbFactory.Create(nameOrConnectionString))
            {
                try
                {
                    foreach (var fileInfo in patchInfo.Files)
                    {
                        var filePath = $"{patchDir}\\{fileInfo.Name}";
                        var sql = File.ReadAllText(filePath, Encoding.UTF8);
                        db.Database.ExecuteSqlCommand(sql);
                    }

                    db.Migrations.Add(new Entities.MigrationInfo
                    {
                        Id = Guid.NewGuid(),
                        Version = patchInfo.To,
                        Files = patchInfo.Files.Select(
                            item =>
                            new Entities.AppliedPatch
                            {
                                Id = Guid.NewGuid(),
                                FileName = item.Name,
                                Hash = item.Hash,
                                Installed = DateTime.Now,
                                Type = item.Type
                            }).ToList(),
                        PatchHash = patchInfo.Hash
                    });
                    db.SaveChanges();

                    scope.Complete();
                }
                catch(Exception ex)
                {
                    return new PatchResult { IsSuccess = false, Message = ex.Message };
                }
            }

            return new PatchResult { IsSuccess = true, Message = "Патч успешно установлен" };
        }

        public static void CreatePath(string name, string path, IEnumerable<string> patchFiles, Version from, Version to)
        {
            if (from >= to)
                throw new Exception($"Не возможно сделать патч с версии {from} до версии {to}");

            if (patchFiles == null || !patchFiles.Any())
                throw new Exception($"Не возможно сделать патч без файлов");

            var patchInfo = new PatchInfo();
            patchInfo.From = from;
            patchInfo.To = to;
            patchInfo.Files = new List<PatchFileInfo>();

            var tempDir = $"{Path.GetTempPath()}{Guid.NewGuid().ToString()}";
            Directory.CreateDirectory(tempDir);

            StringBuilder fileHashes = new StringBuilder();
            fileHashes.Append(from.ToString());
            foreach (var file in patchFiles)
            {
                var info = PrepearFile(tempDir, file);
                patchInfo.Files.Add(info);
                fileHashes.Append(info.Hash);
            }
            fileHashes.Append(to.ToString());

            string patchHashStr = GetPatchHash(fileHashes);
            patchInfo.Hash = patchHashStr;

            var jsonStr = JsonConvert.SerializeObject(patchInfo);
            File.WriteAllText($"{tempDir}\\path_info.json", jsonStr, Encoding.UTF8);
            ZipFile.CreateFromDirectory(tempDir, $"{path}\\{name}.zip");
        }

        private static string GetPatchHash(StringBuilder fileHashes)
        {
            string patchHashStr;
            using (MD5 md5 = MD5.Create())
            {
                var data = Encoding.UTF8.GetBytes(fileHashes.ToString());
                var hash = md5.ComputeHash(data);
                patchHashStr = BitConverter.ToString(hash).Replace("-", "‌​").ToLower();
            }

            return patchHashStr;
        }

        private static PatchFileInfo PrepearFile(string tempDir, string file)
        {
            string fileHashStr;
            if (!File.Exists(file))
                throw new Exception($"Файл не найден. {file}");

            fileHashStr = GetFileHash(file);

            var info = new PatchFileInfo
            {
                Hash = fileHashStr,
                Name = Path.GetFileName(file),
                Type = PatchFileType.SQL
            };

            File.Copy(file, $"{tempDir}\\{info.Name}");
            return info;
        }

        private static string GetFileHash(string file)
        {
            string fileHashStr;
            using (var fStream = File.OpenRead(file))
            using (MD5 md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(fStream);
                fileHashStr = BitConverter.ToString(hash).Replace("-", "‌​").ToLower();
            }

            return fileHashStr;
        }
    }
}
