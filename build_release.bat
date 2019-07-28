@echo on

mkdir bin\

cd BetterMajorasMaskInstaller

msbuild /t:Build /p:Configuration=Release

:: ILMerge doesn't like dotnet core's pdb format
:: https://github.com/dotnet/ILMerge/issues/43
:: so remove them here
del bin\Release\*.pdb

:: let's merge everything
msbuild /t:ILMerge

cd ..\
