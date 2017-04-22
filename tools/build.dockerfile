FROM microsoft/dotnet:1.1.1-sdk

ARG CONFIGURATION=Release
ARG NUGET_PUSH=no
ARG NUGET_SOURCE=https://www.nuget.org/api/v2/package
ARG NUGET_API_KEY=

ENV SOURCES=/opt/sources
ENV ARTIFACTS=/opt/artifacts
ENV CSPROJ=$SOURCES/Teleglib.csproj

ADD *.sln /opt/sources/
ADD Teleglib/*.csproj /opt/sources/Teleglib/
ADD Teleglib.Example/*.csproj /opt/sources/Teleglib.Example/

WORKDIR $SOURCES

RUN dotnet restore

ADD . /opt/sources/
RUN dotnet build -c $CONFIGURATION
RUN dotnet pack -c $CONFIGURATION

RUN mv $SOURCES/Teleglib/bin/$CONFIGURATION $ARTIFACTS

VOLUME /opt/artifacts

RUN if [ $NUGET_PUSH = 'yes' ]; then dotnet nuget push $ARTIFACTS --source $NUGET_SOURCE --api-key $NUGET_API_KEY; fi
