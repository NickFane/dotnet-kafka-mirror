FROM microsoft/dotnet:2-runtime-jessie
ARG source
WORKDIR /app
COPY ${source:-DotnetKafkaMirror/obj/Docker/publish} .
ENTRYPOINT ["dotnet", "DotnetKafkaMirror.dll"]
