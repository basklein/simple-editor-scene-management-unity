# simple-editor-scene-management-unity
Scene Manager for Unity Editor in C#

GitHub: https://github.com/basklein/simple-editor-scene-management-unity

## Summary
This plugin will add a "Scenes" menu to your editor toolbar containing menu items that when selected will quickly open specific scenes from your project.

## Usage
When you add this plugin to your project, you will notice a new option in your editor toolbar: "Scenes". This is where you will find and interact with the features of this plugin.

### Menu Options
<b>Get All (Alt + C)</b> - This option will probably be the most commonly used one. By selecting this option, the plugin scans your project and will find all the scene files in it. It will then generate a menu item for each of the scene files it finds and will place it in the "Scenes" menu.

<b>Reset</b> - This option will revert the settings of the plugin to the default and will remove all the generated menu items.

<b>Settings</b> - This option will open the wizard for the plugin settings. See the Settings section for more information.

<b>Open \<SCENE NAME\></b> - A generated option based on the scene files found by the plugin. Selecting this option will open the associated scene file.

### Settings
<b>Editor Path</b> - The path pointing to the location of the plugin scripts. If for any reason you need to move the scripts from their default location, change the path to point to the new location.

<b>Order Set [string]</b> - A more advanced setting. By entering strings into this array, you can set the order you want the scene menu options to appear in in the "Scenes" menu. By entering a string, the plugin will find the scene files that contain that string in their name and will place them at the desired order index. Files that don't match any string will be clumped together at the bottom of the menu.

<b>Hide Others</b> - When this setting is toggled, any non-matching scene files will be ignored by the plugin.

## Releases

### v1.0.0
Plugin will find all scene files in a Unity project and compile them into a list of menu items. The plugin can filter and sort scenes and create sub-folders based on given strings.

## Background
Originally made to allow for quick scene access in complex Unity projects. Now, I use this plugin in every single project I work on and often forget it's not a standard feature of the Unity editor.
I felt that every Unity user should have access to this tool as it makes scene navigation and management a whole lot easier.

I will at the very least attempt to add or streamline any features if they are requested.

## Credits
Made by Bas Klein.

Based on code by Jeroen Endenburg.

## License
MIT
