﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;

using Microsoft.Templates.Core.Gen;
using Microsoft.Templates.Core.Mvvm;
using Microsoft.Templates.UI.ViewModels.Common;
using Microsoft.Templates.UI.Services;

namespace Microsoft.Templates.UI.ViewModels.NewItem
{
    public abstract class BaseFileViewModel : Observable
    {
        public string DetailTitle { get; protected set; }
        public string DetailDescription { get; protected set; }
        public string Subject { get; protected set; }
        public string Icon { get; private set; }
        public SolidColorBrush CircleColor { get; private set; }
        public FileExtension FileExtension { get; private set; }
        public Func<string, string> UpdateTextAction { get; }

        public string FailedPostaction
        {
            get
            {
                if (FileStatus == FileStatus.ConflictingStylesFile)
                {
                    var name = Subject.Replace(".xaml", "");
                    return Path.Combine(GenContext.Current.OutputPath, $"{name}_failedpostaction.xaml");
                }
                return string.Empty;
            }
        }
        public string TempFile => Path.Combine(GenContext.Current.OutputPath, Subject);
        public string ProjectFile => Path.Combine(GenContext.Current.ProjectPath, Subject);

        private double _codeFontSize;
        public double CodeFontSize
        {
            get => _codeFontSize;
            set => SetProperty(ref _codeFontSize, value);
        }

        public abstract FileStatus FileStatus { get; }

        public virtual string UpdateText(string fileText) => fileText;

        // TODO: Review constructor to remove this suppresion. Important
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BaseFileViewModel(string name)
        {
            Subject = name;
            LoadFile();
            UpdateTextAction = fileText => UpdateText(fileText);
            CodeFontSize = SystemService.Instance.GetCodeFontSize();
        }

        // TODO: Review constructor to remove this suppresion. Important
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BaseFileViewModel(NewItemGenerationFileInfo generationInfo)
        {
            Subject = generationInfo.Name;
            LoadFile();
            UpdateTextAction = fileText => UpdateText(fileText);
            CodeFontSize = SystemService.Instance.GetCodeFontSize();
        }

        private void LoadFile()
        {
            FileExtension = GetFileExtension();
            Icon = GetIcon();
            CircleColor = GetCircleColor();
        }

        public override string ToString()
        {
            return Subject ?? base.ToString();
        }

        private SolidColorBrush GetCircleColor()
        {
            if (Services.SystemService.Instance.IsHighContrast)
            {
                return SystemColors.InfoTextBrush;
            }

            switch (FileStatus)
            {
                case FileStatus.NewFile:
                    return MainViewModel.Current.FindResource<SolidColorBrush>("UIGreen") as SolidColorBrush;
                case FileStatus.ModifiedFile:
                    return MainViewModel.Current.FindResource<SolidColorBrush>("UIBlue") as SolidColorBrush;
                case FileStatus.ConflictingFile:
                    return MainViewModel.Current.FindResource<SolidColorBrush>("UIRed") as SolidColorBrush;
                case FileStatus.ConflictingStylesFile:
                    return MainViewModel.Current.FindResource<SolidColorBrush>("UIDarkYellow") as SolidColorBrush;
                case FileStatus.WarningFile:
                    return MainViewModel.Current.FindResource<SolidColorBrush>("UIDarkYellow") as SolidColorBrush;
                case FileStatus.Unchanged:
                    return MainViewModel.Current.FindResource<SolidColorBrush>("UIDarkBlue");
                default:
                    return new SolidColorBrush(Colors.Transparent);
            }
        }

        private FileExtension GetFileExtension()
        {
            switch (Path.GetExtension(TempFile))
            {
                case ".cs":
                    return FileExtension.CSharp;
                case ".xaml":
                    return FileExtension.Xaml;
                case ".xml":
                    return FileExtension.Xml;
                case ".resw":
                    return FileExtension.Resw;
                case ".csproj":
                    return FileExtension.Csproj;
                case ".appxmanifest":
                    return FileExtension.AppXManifest;
                case ".json":
                    return FileExtension.Json;
                case ".jpg":
                    return FileExtension.Jpg;
                case ".jpeg":
                    return FileExtension.Jpeg;
                case ".png":
                    return FileExtension.Png;
                case ".vb":
                    return FileExtension.Vb;
                case ".vbproj":
                    return FileExtension.Vbproj;
                default:
                    return FileExtension.Default;
            }
        }

        private string GetIcon()
        {
            switch (FileExtension)
            {
                case FileExtension.CSharp:
                    return "/Microsoft.Templates.UI;component/Assets/FileExtensions/CSharp.png";
                case FileExtension.Resw:
                    return "/Microsoft.Templates.UI;component/Assets/FileExtensions/Resw.png";
                case FileExtension.Xaml:
                    return "/Microsoft.Templates.UI;component/Assets/FileExtensions/Xaml.png";
                case FileExtension.Png:
                case FileExtension.Jpg:
                case FileExtension.Jpeg:
                    return "/Microsoft.Templates.UI;component/Assets/FileExtensions/Image.png";
                case FileExtension.Csproj:
                    return "/Microsoft.Templates.UI;component/Assets/FileExtensions/Csproj.png";
                case FileExtension.Json:
                    return "/Microsoft.Templates.UI;component/Assets/FileExtensions/Json.png";
                case FileExtension.Vb:
                    return "/Microsoft.Templates.UI;component/Assets/FileExtensions/VisualBasic.png";
                case FileExtension.Vbproj:
                    return "/Microsoft.Templates.UI;component/Assets/FileExtensions/VBProj.png";
                default:
                    return "/Microsoft.Templates.UI;component/Assets/FileExtensions/DefaultFile.png";
            }
        }
    }
}
