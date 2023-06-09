﻿namespace App.Models
{
    public class Summernote
    {
        public Summernote(string idEditor, bool loadLibrary = true)
        {
            IDEditor = idEditor;
            LoadLibrary = loadLibrary;
        }
        public string IDEditor { get; set; }
        public bool LoadLibrary { get; set; }
        public int height { get; set; } = 400;
        public string toolbar { get; set; } = @"
         [
          ['style', ['style']],
          ['font', ['bold', 'underline', 'clear']],
          ['color', ['color']],
          ['para', ['ul', 'ol', 'paragraph']],
          ['table', ['table']],
          ['insert', ['link', 'picture', 'video', 'elfinder']],
           ['height', ['height']],
          ['view', ['fullscreen', 'codeview', 'help']]
        ]
        ";
    }
}