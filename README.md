# Blog Module

A sample module demonstrating how to create an external module that is integrated with the Oqtane framework at runtime. This module uses the same approach as the external template which is available via the Module Creator within the framework and can be used to scaffold new modules.

The module is a very basic blog. It allows an authorized user to create blog entries including title, summary, and description. It provides custom templating capabilities so that you can customize the display to suit your needs. The module is being used on https://www.oqtane.org/blog.

Note that you cannot run this module directly in your IDE. You need to ensure that the DNF.Projects folder is located within the same parent folder as the Oqtane framework:  

```
/parent
  /Oqtane.Blogs
  /oqtane.framework
```

Organizing the folders in this way allows the system to automatically deploy the module DLLs to the Oqtane framework when your build the module solution. Then you can run the Oqtane framework and it will dynamically load the module.


# Example Screenshots

Various renderings of the Blog module:

![Module](https://github.com/oqtane/oqtane.blogs/blob/master/screenshot1.png?raw=true "Module")

![Module](https://github.com/oqtane/oqtane.blogs/blob/master/screenshot2.png?raw=true "Module")

![Module](https://github.com/oqtane/oqtane.blogs/blob/master/screenshot3.png?raw=true "Module")

![Module](https://github.com/oqtane/oqtane.blogs/blob/master/screenshot4.png?raw=true "Module")
