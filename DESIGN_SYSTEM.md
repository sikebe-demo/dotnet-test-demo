# Design System Documentation

This document outlines the unified design system implemented across the RazorPagesProject application to ensure consistent UI/UX.

## Color Palette

### Primary Colors
- **Primary Color**: `#2b5876` - Main brand color
- **Secondary Color**: `#4e4376` - Complementary brand color
- **Accent Color**: `#0077cc` - Link and accent color

### Semantic Colors
- **Success**: `#28a745`
- **Danger**: `#dc3545`
- **Warning**: `#ffc107`
- **Info**: `#17a2b8`

### Gradients
- **Primary Gradient**: `linear-gradient(135deg, #2b5876 0%, #4e4376 100%)`
- **Primary Gradient Reverse**: `linear-gradient(135deg, #4e4376 0%, #2b5876 100%)`

## Typography

### Headings
- **Page Title**: Gradient text effect using `.page-title` class
- **Card Titles**: White text on gradient background in card headers

## Component System

### Cards
- **Base Class**: `.card` - Enhanced Bootstrap cards with shadows and hover effects
- **Unified Cards**: `.unified-card` - Custom card system with consistent styling
- **Card Headers**: `.card-header` - Gradient background with white text

### Buttons
- **Primary**: `.btn-primary` - Gradient background with hover effects
- **Unified Primary**: `.btn-unified-primary` - Rounded buttons with gradient
- **Unified Secondary**: `.btn-unified-secondary` - Outlined buttons that fill on hover
- **Unified Danger**: `.btn-unified-danger` - Smaller danger buttons for actions

### Forms
- **Unified Form Controls**: `.form-control-unified` - Rounded inputs with custom focus states

## Design Tokens (CSS Custom Properties)

```css
:root {
  /* Colors */
  --primary-color: #2b5876;
  --secondary-color: #4e4376;
  --accent-color: #0077cc;
  
  /* Gradients */
  --primary-gradient: linear-gradient(135deg, var(--primary-color) 0%, var(--secondary-color) 100%);
  --primary-gradient-reverse: linear-gradient(135deg, var(--secondary-color) 0%, var(--primary-color) 100%);
  
  /* Shadows */
  --card-shadow: 0 10px 20px rgba(0,0,0,0.1);
  --button-shadow: 0 5px 15px rgba(0,0,0,0.1);
  
  /* Border Radius */
  --border-radius-sm: 8px;
  --border-radius-md: 15px;
  --border-radius-lg: 30px;
  
  /* Spacing */
  --spacing-xs: 0.5rem;
  --spacing-sm: 1rem;
  --spacing-md: 1.5rem;
  --spacing-lg: 2rem;
  --spacing-xl: 3rem;
}
```

## Animation and Effects

### Hover Effects
- **Cards**: Subtle lift effect with `transform: translateY(-2px)`
- **Buttons**: Lift effect with enhanced shadow
- **Enhanced shadows**: More prominent shadows on hover

### Animations
- **Fade In**: `.fade-in` class for smooth page entry animations
- **Transitions**: 0.3s ease transitions on interactive elements

## Responsive Design

### Breakpoints
- **Mobile**: < 768px - Adjusted padding and button sizes
- **Tablet**: 768px - 1024px - Maintains desktop layout with optimized spacing
- **Desktop**: > 1024px - Full design system applied

### Mobile Adaptations
- Reduced card padding
- Smaller button sizes
- Optimized font sizes

## Usage Guidelines

### When to Use Each Component
1. **`.card`**: For any grouped content that needs visual separation
2. **`.btn-primary`**: For main call-to-action buttons
3. **`.btn-unified-secondary`**: For secondary actions
4. **`.btn-unified-danger`**: For destructive actions (delete, clear)
5. **`.page-title`**: For main page headings to maintain visual hierarchy

### Consistency Rules
1. Always use gradient backgrounds for card headers
2. Maintain consistent spacing using the CSS custom properties
3. Apply hover effects to interactive elements
4. Use icons from FontAwesome for visual consistency
5. Ensure proper contrast ratios for accessibility

## File Structure

- **`wwwroot/css/site.css`**: Main design system definitions
- **`Pages/Shared/_Layout.cshtml.css`**: Layout-specific enhancements
- **`Pages/GitHubProfile.cshtml.css`**: Page-specific extensions (preserved for advanced styling)

## Maintenance

When adding new components:
1. Follow the established color palette
2. Use existing CSS custom properties
3. Maintain consistent hover and transition effects
4. Test responsive behavior
5. Ensure accessibility standards are met

## Browser Support

The design system uses modern CSS features including:
- CSS Custom Properties (CSS Variables)
- CSS Grid and Flexbox
- Modern box-shadow and transitions
- Gradient backgrounds

Supports all modern browsers (Chrome, Firefox, Safari, Edge).