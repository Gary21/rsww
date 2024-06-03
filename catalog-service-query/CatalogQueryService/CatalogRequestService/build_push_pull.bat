@echo off
REM Set variables
set IMAGE_NAME=rsww-catalog-query-service
set IMAGE_TAG=latest
set DOCKER_HUB_USERNAME=woobin000

REM Build the Docker image
echo Building the Docker image...
REM docker build -t %IMAGE_NAME%:%IMAGE_TAG% .
docker build -t %IMAGE_NAME% .

REM Tag the Docker image
echo Tagging the Docker image...
REM docker tag %IMAGE_NAME%:%IMAGE_TAG% %DOCKER_HUB_USERNAME%/%IMAGE_NAME%:%IMAGE_TAG%
docker tag %IMAGE_NAME% %DOCKER_HUB_USERNAME%/%IMAGE_NAME%

REM Push the Docker image to Docker Hub
echo Pushing the Docker image to Docker Hub...
REM docker push %DOCKER_HUB_USERNAME%/%IMAGE_NAME%:%IMAGE_TAG%
docker push %DOCKER_HUB_USERNAME%/%IMAGE_NAME%

echo Pushed!

REM update local image
echo Updating local image in 10 seconds ...
timeout /t 10
REM docker pull %DOCKER_HUB_USERNAME%/%IMAGE_NAME%:%IMAGE_TAG%
docker pull %DOCKER_HUB_USERNAME%/%IMAGE_NAME%

echo Done!