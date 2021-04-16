# Epitech Dashboard ![CI](https://github.com/EpitechIT2020/B-DEV-500-BDX-5-1-cardgames-valentin.charbonnier/workflows/CI/badge.svg)

Epitech Dashboard is a web application developed with ASP.NET and Bootstrap that allows you to quickly visualize your information according to your subscribed services.

You can see the architecture documentation in Documentation folder.

## Prerequisites
- [Docker](https://docs.docker.com/docker-for-windows/install/)

## Installation
Launch the server locally using docker

```bash
docker-compose build && docker-compose up
```

## Usage
Once the server is launched connect to [localhost:8080](http://localhost:8080/)

## Services & Widgets
### Intranet

Allow you to get information about Epitech Intranet

- **Notes** -> Display your current GPA and credits
- **Notifications** -> Display your latest Intranet notifications

### Github

Allow you to get information about your Github account

- **Notification** -> Display your latest Gitbub notifications

### IP

Allow you to get information about your public IP address

- **Show my infos** -> Display your public IP address, City, Region, country and postal code 

### CatAPI

Random cat images

- **Cat** -> Display random images of cats
