@echo on

mkdir bin
:: clean, build and merge dlls
cd "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin"
MSBuild.exe /t:Clean /t:Build /t:ILMerge ^
	/p:Configuration=Release ^
	/p:AllowedReferenceRelatedFileExtensions=none ^
	%UserProfile%\source\repos\BetterMajorasMaskInstaller\BetterMajorasMaskInstaller\BetterMajorasMaskInstaller.csproj
pause
:: Generate MD5 Hash
cd "C:\Windows\System32"
CertUtil.exe -hashfile %UserProfile%\source\repos\BetterMajorasMaskInstaller\bin\BetterMajorasMaskInstaller.exe MD5
pause
