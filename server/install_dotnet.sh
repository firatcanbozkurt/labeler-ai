#!/bin/bash

echo "Installing .NET 6.0..."

# Add Microsoft package repository
wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# Update package list
sudo apt-get update

# Install .NET 6.0 SDK
sudo apt-get install -y apt-transport-https
sudo apt-get update
sudo apt-get install -y dotnet-sdk-6.0

# Verify installation
dotnet --version

echo "Installation complete!"
echo "You can now run: ./build.sh"