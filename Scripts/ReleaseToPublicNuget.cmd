Echo Creates Release Packages

set packages="..\packages\release"

set program="..\src\UniqueKeys"
dotnet msbuild /p:Configuration=Release %program%
dotnet pack -o %packages% %program%

