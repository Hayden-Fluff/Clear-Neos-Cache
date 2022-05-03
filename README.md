# Clear Neos Cache

A small application to delete any files that haven't been read in a definied amount of days in a given directory, meant for automated use with NeosVR

## Usage

`ClearNeosCache.exe -a <int> [-d <string>]`

-a --age This tells the program to delete any files that have not been accessed for x amount of days, any value lower than 0 will throw an exception

-d --directory    [Optional] This tells the program where to look, an invalid directory will throw an exception.
If no directory is given, then it will default to `%temp%\Solarix\NeosVR\Cache`

## Disclaimer

This program **will** permanently remove files in the given directory with the only confirmation being running the program, use outside of an automated schedule is not advised.

The program will only run in Windows NT systems, as it uses methods that only work on Windows NT platforms
Launching the executable on another OS will throw an exception.

## How to use

The suggested method is by using the Windows Task Scheduler, though if you have a method of running an application with launch arguments that will work just as well.


## External Libraries

Command Line Parser: https://github.com/commandlineparser/commandline

Used in parsing command line arguments from a CLI application