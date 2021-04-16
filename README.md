<h1 align="center">
  <br>
  <a href=""><img src="assets/logo.png" alt="fred" width="150"></a>
  <br>
  Clockodo automation
  <br>
</h1>

<p align="center">
  <a href="https://github.com/Azure/bicep">
    <img src="https://img.shields.io/static/v1?label=Azure-Bicep&message=v0.2.59&style=for-the-badge&logo&color=brightgreen">
  </a>
  <a href="https://docs.microsoft.com/en-us/azure/azure-functions/">
    <img src="https://img.shields.io/static/v1?label=Azure-FUNC%20&message=v3.0&style=for-the-badge&color=blue">
  </a>
  <a href="https://github.com/dotnet/core">
    <img src="https://img.shields.io/static/v1?label=.net%20&message=v3.1&style=for-the-badge&color=blue">
  </a>
  <a href="https://github.com/whiteducksoftware/clockodo-automation/blob/master/LICENSE">
    <img src="https://img.shields.io/static/v1?label=LICENSE&message=MIT&style=for-the-badge&color=brightgreen">
  </a>
</p>

---

<p align="center">
  <img src="assets/clockodo-backup-solution_transparent.png" width="700"/>
</p>

---
## Try it out

[![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2F55e62e20a04044a3c2354d2960425952.m.pipedream.net%2F)

A small solution for creating automated backups of your Clockodo entries. The "Deploy to Azure" button will create a small environment consisting of: 
- Azure Key vault
- Storage account with blob container
- App Service plan
- Function App 
- Application Insights

By default, the deployment will create two functions that will perform backups on a daily and monthly basis. 

### Prerequisites
- Azure Subscription
- E-mail address of a Clockodo API User
- Clockodo API key

## The purpose of this project are two things:

- Provide additional features around clockodo like automated backup, analytics or alerting.
- Serve as a learning environment for white duck 🦆 employees. Quack quack

## Technologies

- Azure Biceps 💪 to generate ARM templates
- DevContainer

---
