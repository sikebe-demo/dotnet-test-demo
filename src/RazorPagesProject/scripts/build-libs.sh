#!/bin/bash

echo "📦 Building client-side libraries..."

echo "📁 Creating output directories..."
mkdir -p wwwroot/lib/bootstrap/dist/css
mkdir -p wwwroot/lib/bootstrap/dist/js
mkdir -p wwwroot/lib/jquery/dist
mkdir -p wwwroot/lib/jquery-validation/dist
mkdir -p wwwroot/lib/jquery-validation-unobtrusive

# Bootstrap
echo "🎨 Copying Bootstrap files..."
cp node_modules/bootstrap/dist/css/bootstrap.min.css wwwroot/lib/bootstrap/dist/css/
cp node_modules/bootstrap/dist/js/bootstrap.bundle.min.js wwwroot/lib/bootstrap/dist/js/

# jQuery
echo "💫 Copying jQuery files..."
cp node_modules/jquery/dist/jquery.min.js wwwroot/lib/jquery/dist/

# jQuery Validation
echo "✅ Copying jQuery Validation files..."
cp node_modules/jquery-validation/dist/jquery.validate.min.js wwwroot/lib/jquery-validation/dist/

# jQuery Validation Unobtrusive
echo "🔗 Copying jQuery Validation Unobtrusive files..."
cp node_modules/jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.min.js wwwroot/lib/jquery-validation-unobtrusive/

echo "✅ Client-side libraries build completed!"
