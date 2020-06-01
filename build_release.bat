@echo on

mkdir bin
:: clean, build and merge dlls
msbuild /t:Clean /t:Build /t:ILMerge ^
	/p:Configuration=Release ^
	/p:AllowedReferenceRelatedFileExtensions=none ^
	BetterMajorasMaskInstaller\BetterMajorasMaskInstaller.csproj

:: Generate MD5 Hash
CertUtil -hashfile bin\BetterMajorasMaskInstaller.exe MD5