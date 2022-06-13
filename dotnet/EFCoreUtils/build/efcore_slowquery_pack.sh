echo "Start packing"
version=$1
echo "package version is $version"
dotnet pack -p:Version=$version ../src/EFCoreSlowQuery/EFCoreSlowQuery.csproj --include-symbols  --output ../pkgs/

unset version
