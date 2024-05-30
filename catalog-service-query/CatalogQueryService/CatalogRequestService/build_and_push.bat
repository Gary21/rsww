@echo off
REM Set variables
set IMAGE_NAME=rsww-catalog-query-service
set IMAGE_TAG=1.0
set DOCKER_HUB_USERNAME=woobin000

REM Build the Docker image
echo Building the Docker image...
docker build -t %IMAGE_NAME%:%IMAGE_TAG% .

REM Tag the Docker image
echo Tagging the Docker image...
docker tag %IMAGE_NAME%:%IMAGE_TAG% %DOCKER_HUB_USERNAME%/%IMAGE_NAME%:%IMAGE_TAG%

REM Push the Docker image to Docker Hub
echo Pushing the Docker image to Docker Hub...
docker push %DOCKER_HUB_USERNAME%/%IMAGE_NAME%:%IMAGE_TAG%

echo Pushed!

REM update local image
echo Updating local image in 5 seconds ...
timeout /t 5
docker pull %DOCKER_HUB_USERNAME%/%IMAGE_NAME%:%IMAGE_TAG%

echo Done!