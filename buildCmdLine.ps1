
dotnet restore ./MilleniumFalconChallenge/MilleniumFalconChallenge.sln

dotnet build ./MilleniumFalconChallenge/MFC.CmdLine/MFC.CmdLine.csproj -r linux-x64 -c Release -p:PublishSingleFile=true
dotnet build ./MilleniumFalconChallenge/MFC.CmdLine/MFC.CmdLine.csproj -r win-x64 -c Release -p:PublishSingleFile=true
dotnet build ./MilleniumFalconChallenge/MFC.CmdLine/MFC.CmdLine.csproj -r osx-x64 -c Release -p:PublishSingleFile=true
dotnet build ./MilleniumFalconChallenge/MFC.CmdLine/MFC.CmdLine.csproj -r osx-arm64 -c Release -p:PublishSingleFile=true

dotnet publish ./MilleniumFalconChallenge/MFC.CmdLine/MFC.CmdLine.csproj -r linux-x64 -c Release -p:PublishSingleFile=true --self-contained --no-build --no-restore -o ./build/linux-x64
dotnet publish ./MilleniumFalconChallenge/MFC.CmdLine/MFC.CmdLine.csproj -r win-x64 -c Release -p:PublishSingleFile=true --self-contained --no-build --no-restore -o ./build/win-x64
dotnet publish ./MilleniumFalconChallenge/MFC.CmdLine/MFC.CmdLine.csproj -r osx-x64 -c Release -p:PublishSingleFile=true --self-contained --no-build --no-restore -o ./build/osx-x64
dotnet publish ./MilleniumFalconChallenge/MFC.CmdLine/MFC.CmdLine.csproj -r osx-arm64 -c Release -p:PublishSingleFile=true --self-contained --no-build --no-restore -o ./build/osx-arm64