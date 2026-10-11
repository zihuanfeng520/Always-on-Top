@echo off
rem 用 Windows 內建的 .NET Framework 編譯器重新編譯
set CSC=%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe
"%CSC%" /target:winexe /optimize+ /win32icon:icon.ico /resource:icon96.png,icon96.png /reference:System.Windows.Forms.dll /reference:System.Drawing.dll /out:Always-on-Top.exe Always-on-Top.cs
pause
