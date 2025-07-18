#!/bin/bash

echo "🚀 Dev Container setup starting..."

echo "📝 Configuring Git safe directory..."
git config --global --add safe.directory ${containerWorkspaceFolder}

echo "📦 Updating npm to latest version..."
npm install -g npm@latest

echo "📦 Installing npm dependencies for RazorPagesProject..."
cd src/RazorPagesProject
npm install

echo "✅ Dev Container setup completed successfully!"
