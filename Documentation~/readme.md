# Automated Documentation

## Setup
1) Install [DocFX](https://github.com/dotnet/docfx) if you haven't already. (Use version 2.61.0 if building the metadata doesn't work) 
2) cd into 'Documentation'
3) Run `docfx metadata` to generate the metadata files.
4) Run `docfx build` to generate the documentation.
5) Run `docfx serve output` to serve the documentation locally.