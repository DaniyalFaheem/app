# Image Assets Usage Guide

## Overview
This document describes how the image assets are organized and used in the Face Recognition Attendance Management System.

## Image Assets

### 1. Background Image (`background.jpg`)
- **Location**: `src/Resources/Images/background.jpg`
- **Type**: Futuristic tech-themed background with green/teal lighting effects
- **Usage**: Applied as a subtle background in windows for a modern look
- **Implementation**:
  - Applied to `UserManagementWindow.xaml` with 15% opacity
  - Applied to `EditUserWindow.xaml` with 12% opacity

**Example XAML:**
```xml
<Grid.Background>
    <ImageBrush ImageSource="/Resources/Images/background.jpg" 
               Opacity="0.15" 
               Stretch="UniformToFill"/>
</Grid.Background>
```

### 2. Send Button Image (`send_button.png`)
- **Location**: `src/Resources/Images/send_button.png`
- **Type**: Green "SEND" button with rounded corners
- **Usage**: Custom button style for send/submit actions (e.g., WhatsApp alerts)
- **Implementation**: Available as `SendButtonStyle` in both windows

**How to Use the Send Button Style:**

In any XAML file where you want to use the send button image:

```xml
<!-- Option 1: As a clickable button -->
<Button Style="{StaticResource SendButtonStyle}"
        Command="{Binding YourSendCommand}"
        Width="120"
        Height="50"
        ToolTip="Send WhatsApp Alert">
</Button>

<!-- Option 2: In action buttons area -->
<StackPanel Orientation="Horizontal">
    <Button Style="{StaticResource SendButtonStyle}"
            Command="{Binding SendAlertCommand}"
            Width="100"
            Height="45"
            Margin="0,0,10,0"/>
    <Button Content="Other Action"
            Width="130"
            Height="40"/>
</StackPanel>
```

**Button Features:**
- Hover effect: 80% opacity on mouse over
- Press effect: 60% opacity when clicked
- Disabled state: 40% opacity when disabled
- Transparent background and borders
- Hand cursor on hover

## Best Practices

1. **Background Opacity**: Keep the background opacity low (10-20%) to ensure text readability
2. **Button Size**: Recommended size for send button: 80-120px width, 40-60px height
3. **Accessibility**: Always add ToolTip to custom image buttons for accessibility
4. **Consistent Style**: Use the SendButtonStyle across all windows for consistency

## Example: Adding Send Alert Button

If you want to add a "Send WhatsApp Alert" button to a window:

1. Ensure the window has access to the SendButtonStyle (copy from UserManagementWindow.xaml resources if needed)
2. Add the button in your layout:

```xml
<Button Style="{StaticResource SendButtonStyle}"
        Command="{Binding SendWhatsAppAlertCommand}"
        Width="110"
        Height="48"
        ToolTip="Send WhatsApp notification to selected users">
</Button>
```

3. Implement the command in your ViewModel:

```csharp
public ICommand SendWhatsAppAlertCommand { get; }

// In constructor:
SendWhatsAppAlertCommand = new RelayCommand(
    execute: () => SendWhatsAppAlert(),
    canExecute: () => SelectedUser != null
);

private void SendWhatsAppAlert()
{
    _whatsAppService.SendAbsenteeAlert(SelectedUser, DateTime.Now);
}
```

## Technical Details

- **Resource Type**: Images are marked as `Resource` in the .csproj file for WPF pack URI access
- **Pack URI Format**: `/Resources/Images/filename.ext`
- **Build Action**: Resource (automatically set by wildcard pattern in .csproj)

## Future Enhancements

Consider adding these image assets in the future:
- Icon for camera/face recognition
- Icon for attendance marking
- Icon for reports/exports
- Loading animations
- Status indicators (success/error)
