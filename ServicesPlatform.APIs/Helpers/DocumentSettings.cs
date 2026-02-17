using System;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace ServicesPlatform.APIs.Helpers
{
    public class DocumentSettings
    {
        public static string UploadFile(IFormFile file, string folderName)
        {
            // 1. تحديد مسار المجلد بشكل ديناميكي لأي نظام تشغيل
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", folderName);

            // إنشاء المجلد إذا لم يكن موجود
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            // 2. إنشاء اسم فريد للملف مع الحفاظ على الامتداد
            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            // 3. مسار الملف الكامل
            string filePath = Path.Combine(folderPath, fileName);

            // 4. حفظ الملف
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            return fileName;
        }

        public static void DeleteFile(string fileName, string folderName)
        {
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", folderName);
            string filePath = Path.Combine(folderPath, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
