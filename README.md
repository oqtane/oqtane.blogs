# Latest Release

[6.2.0](https://github.com/oqtane/oqtane.blogs/releases/tag/v6.2.0) was released on Sep 18, 2025.

**Please Note** this module must be used with Oqtane 6.2.0 or higher as it now uses a Nuget package with a standard nuspec and static asset approach

# Blog Module

This module allows an authorized user to create blog entries including title, summary, and description. Content can contain HTML formatting, links, and images. The module provides custom templating capabilities so that you can customize the display to suit your needs. It has search functionality so that users can easily locate a specific blog by keyword. It supports site maps so that the blog entries are indexable by search engines. It allows users to subscribe and be notified by email when new blogs are published (unsubscribe is also supported to comply with spam regulations). An RSS feed is also available for content syndication. 

The module is being used on https://www.oqtane.org/blog (as well as other production sites)

Note that you need to ensure that the folder is organized as a sibling of the Oqtane framework in your local environment (ie. it is located within the same parent folder):  

```
/parent
  /Oqtane.Blogs
  /Oqtane.Framework
```

Organizing the folders in this way allows the system to automatically deploy the module DLLs to the Oqtane framework when your build the module solution.

# Example Screenshots

Various renderings of the Blog module:

![Module](https://github.com/oqtane/oqtane.blogs/blob/master/screenshot1.png?raw=true "Module")

![Module](https://github.com/oqtane/oqtane.blogs/blob/master/screenshot2.png?raw=true "Module")

![Module](https://github.com/oqtane/oqtane.blogs/blob/master/screenshot3.png?raw=true "Module")

![Module](https://github.com/oqtane/oqtane.blogs/blob/master/screenshot4.png?raw=true "Module")
