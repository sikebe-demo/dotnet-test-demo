const fs = require('fs');
const path = require('path');

console.log('📦 Building client-side libraries...');

function ensureDirectoryExists(dirPath) {
    if (!fs.existsSync(dirPath)) {
        fs.mkdirSync(dirPath, { recursive: true });
    }
}

function copyFile(src, dest) {
    const destDir = path.dirname(dest);
    ensureDirectoryExists(destDir);
    fs.copyFileSync(src, dest);
}

console.log('📁 Creating output directories...');

// Create output directories
const directories = [
    'wwwroot/lib/bootstrap/dist/css',
    'wwwroot/lib/bootstrap/dist/js',
    'wwwroot/lib/jquery/dist',
    'wwwroot/lib/jquery-validation/dist',
    'wwwroot/lib/jquery-validation-unobtrusive'
];

directories.forEach(dir => ensureDirectoryExists(dir));

try {
    // Bootstrap
    console.log('🎨 Copying Bootstrap files...');
    copyFile(
        'node_modules/bootstrap/dist/css/bootstrap.min.css',
        'wwwroot/lib/bootstrap/dist/css/bootstrap.min.css'
    );
    copyFile(
        'node_modules/bootstrap/dist/js/bootstrap.bundle.min.js',
        'wwwroot/lib/bootstrap/dist/js/bootstrap.bundle.min.js'
    );

    // jQuery
    console.log('💫 Copying jQuery files...');
    copyFile(
        'node_modules/jquery/dist/jquery.min.js',
        'wwwroot/lib/jquery/dist/jquery.min.js'
    );

    // jQuery Validation
    console.log('✅ Copying jQuery Validation files...');
    copyFile(
        'node_modules/jquery-validation/dist/jquery.validate.min.js',
        'wwwroot/lib/jquery-validation/dist/jquery.validate.min.js'
    );

    // jQuery Validation Unobtrusive
    console.log('🔗 Copying jQuery Validation Unobtrusive files...');
    copyFile(
        'node_modules/jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.min.js',
        'wwwroot/lib/jquery-validation-unobtrusive/jquery.validate.unobtrusive.min.js'
    );

    console.log('✅ Client-side libraries build completed!');
} catch (error) {
    console.error('❌ Error copying files:', error.message);
    process.exit(1);
}
