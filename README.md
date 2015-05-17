# ShinyMusicSynchronizer

ShinyMusicSynchronizer is a tool designed for one-way synchronization of music library from my computer to my phone. Due to this, I left in some unfinished things that are noncritical:

* While the parameters can be supplied through command-line, I can't get the clipr library to output the usage help. This is not much of a problem, because if you have only one device, you should put the configuration into the ShinyMusicSynchronizer.exe.config file.

This tool is somewhat overengineered. I've wanted to try some new things for some time and it seemed like a good opportunity. If anyone wants to extend the code (for example add a GUI), open an issue (on GitHub), or write me an e-mail (less reliable).

## How it works

You specify the root path for the device and the computer. The tool then compares what files are present where (only names are compared cuerrently due to the wacky way MTP protocol reports file modification date) and the tool displays the proposed changes and **starts doing them**.

You will see the progress of copy operations. This is designed for MP3 music files, so if you synchronize anything big, you will get spammed.
