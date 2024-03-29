# See here for image contents: https://github.com/microsoft/vscode-dev-containers/tree/v0.140.1/containers/ubuntu/.devcontainer/base.Dockerfile
ARG VARIANT

FROM mcr.microsoft.com/vscode/devcontainers/base:0-${VARIANT}

ARG VARIANT
ARG DOTNET_VERSION=3.1
ARG NODE_VERSION=12

ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

RUN apt-get update \
  && apt-get install -y software-properties-common build-essential \
  && add-apt-repository ppa:longsleep/golang-backports \
  && add-apt-repository universe \
  \
  # Change default shell to zsh
  && chsh -s $(which zsh) \
  \
  # Add dotnet repo
  && wget https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb -O packages-microsoft-prod.deb \
  && curl -sS https://dl.yarnpkg.com/debian/pubkey.gpg | sudo apt-key add - \
  && echo "deb https://dl.yarnpkg.com/debian/ stable main" | sudo tee /etc/apt/sources.list.d/yarn.list \
  && dpkg -i packages-microsoft-prod.deb \
  \
  # Add azure repo
  && curl -sL https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor | sudo tee /etc/apt/trusted.gpg.d/microsoft.gpg > /dev/null \
  && echo "deb [arch=amd64] https://packages.microsoft.com/repos/azure-cli/ $(lsb_release -cs) main" | sudo tee /etc/apt/sources.list.d/azure-cli.list \
  \
  # Add node 14 repo
  && curl -sSL https://deb.nodesource.com/gpgkey/nodesource.gpg.key | sudo apt-key add - \
  && echo "deb https://deb.nodesource.com/node_${NODE_VERSION}.x $(lsb_release -cs) main" | sudo tee /etc/apt/sources.list.d/nodesource.list \
  && echo "deb-src https://deb.nodesource.com/node_${NODE_VERSION}.x $(lsb_release -cs) main" | sudo tee -a /etc/apt/sources.list.d/nodesource.list \
  \
  # Add yarn repo
  && curl -sS https://dl.yarnpkg.com/debian/pubkey.gpg | sudo apt-key add - \
  && echo "deb https://dl.yarnpkg.com/debian/ stable main" | sudo tee /etc/apt/sources.list.d/yarn.list \
  \
  && apt-get update \
  && apt install -y build-essential \
    dotnet-sdk-${DOTNET_VERSION} dotnet-runtime-${DOTNET_VERSION} aspnetcore-runtime-${DOTNET_VERSION} \
    powershell \
    azure-cli \
    nodejs yarn \
    azure-functions-core-tools-3 \
  \
  # Install npm dependencies
  && yarn global add @angular/cli \
  \
  && apt-get clean \
  && rm -rf packages-microsoft-prod.deb /var/lib/{apt,dpkg,cache,log}/ 