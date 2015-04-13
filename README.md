# Flirper
Flirper mod for Cities: Skylines

This mod changes the main menu background to a random image from a user-supplied list.

#Usage
##Getting started
Subscribe to the mod via the Steam Workshop (or put it in your local Mods if you are compiling manually). 

Once added, it will read (and create if needed) the file FlirperImageList.txt in your Cities: Skylines user folder:
* Windows: `C:\Users\<username>\AppData\Local\Colossal Order\Cities_Skylines\ModConfig\`
* Mac: `/Users/<username>/Library/Application Support/Colossal Order/Cities_Skylines/ModConfig/`
* Linux: `/home/<username>/.local/share/Colossal Order/Cities_Skylines/ModConfig/`

##Default configuration
The mod ships with a default FlirperImageList.txt. It contains 10 links to Creative Commons-licensed images with attributions to serve as an example.

##Configuration
FlirperImageList.txt is a list of image sources, with each line representing one source. Each line must start with a path to either a local image file, a local directory or a URL to an image. For example:
```
C:\Users\MyUserName\Desktop\images\
C:\Users\MyUserName\Desktop\family\cat.jpg
http://i.imgur.com/H2mby53.jpg
```

Each line (except for directories) can have additional fields separated by a `;`. The fields are: 
* path
* title
* author
* attribution

For example
```
http://i.imgur.com/H2mby53.jpg;Cats on stairs;13ucci;http://redd.it/2ytelo/
```

##Disabling the mod
Flirper cannot be enabled or disabled via the Content Manager, because its logic is started when the game checks the mod's name. Therefore, to disable Flirper, you need to unsubscribe from the mod (through Steam or the Content Manager).

#Known issues
Flirper should fail gracefully when it cannot load an image, but I have not tested every possible combination. Similarly, I don't know how it handles uncommon resolutions.

Some of the default image URLs may not be reachable at any moment. Flirper will write an exception to the game's console (F7) when it can't load a source.

#Attributions
The default images are fully sourced in the FlirperImageList.txt file.

Thanks to the people of `#skylines-modders @ irc.esper.net` for many ideas and solutions.
