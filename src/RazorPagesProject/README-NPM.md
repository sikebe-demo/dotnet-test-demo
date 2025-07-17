# Client-Side Package Management with npm

This document describes how client-side packages are managed in the RazorPagesProject using npm.

## Overview

The project now uses npm to manage client-side dependencies instead of manually maintaining library files. This provides better dependency management, easier updates, and cleaner source control.

## Dependencies

The following client-side packages are managed via npm:

- **Bootstrap 5.3.7** - CSS framework for responsive design
- **jQuery 3.7.1** - JavaScript library for DOM manipulation and AJAX
- **jQuery Validation 1.21.0** - Client-side form validation
- **jQuery Validation Unobtrusive 4.0.0** - ASP.NET Core integration for jQuery Validation

## Setup

### Prerequisites

- Node.js and npm installed on your development machine

### Initial Setup

1. Navigate to the project directory:
   ```bash
   cd src/RazorPagesProject
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

3. Build client-side assets:
   ```bash
   npm run build
   ```

## npm Scripts

The project includes the following npm scripts:

- `npm run build` - Copy all client-side packages to wwwroot/lib
- `npm run copy-libs` - Copy all libraries (alias for build)
- `npm run copy-bootstrap` - Copy Bootstrap files only
- `npm run copy-jquery` - Copy jQuery files only
- `npm run copy-jquery-validation` - Copy jQuery Validation files only
- `npm run copy-jquery-validation-unobtrusive` - Copy jQuery Validation Unobtrusive files only

## File Structure

After running `npm run build`, the following structure is maintained in `wwwroot/lib`:

```
wwwroot/lib/
├── bootstrap/
│   └── dist/
│       ├── css/
│       └── js/
├── jquery/
│   └── dist/
├── jquery-validation/
│   └── dist/
└── jquery-validation-unobtrusive/
    ├── jquery.validate.unobtrusive.js
    ├── jquery.validate.unobtrusive.min.js
    └── LICENSE.txt
```

## Integration with .NET Build

The npm packages are automatically copied to the appropriate locations when you run:
- `npm install` (via postinstall script)
- `npm run build`

## Development Workflow

1. **Adding new packages**: Use `npm install <package-name>` and update the copy scripts in package.json
2. **Updating packages**: Use `npm update` and run `npm run build`
3. **Removing packages**: Use `npm uninstall <package-name>` and remove corresponding copy scripts

## Gitignore

The following npm-related files are excluded from source control:
- `node_modules/`
- `npm-debug.log*`
- `yarn-debug.log*`
- `yarn-error.log*`

## Migration Notes

This setup maintains compatibility with existing file references in:
- `Pages/Shared/_Layout.cshtml`
- `Pages/Shared/_ValidationScriptsPartial.cshtml`

The file paths remain the same, ensuring no changes are needed to existing Razor pages.