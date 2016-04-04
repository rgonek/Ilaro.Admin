Ilaro.Admin [![Build status](https://ci.appveyor.com/api/projects/status/a1kfg9eig0i7cer1?svg=true)](https://ci.appveyor.com/project/rgonek/ilaro-admin)
===========

Ilaro.Admin creates for you admin panel using only POCO classes.

[Demo](http://admin.ilaro.net/) - using Northwind DB (with small modifications, removed multiple primary keys)

Project was inspired by [Django admin site](https://docs.djangoproject.com/en/dev/ref/contrib/admin/).

Please keep in mind this is still an alpha version.

Get it from nuget:

If you are using Unity install:
```
Install-Package Ilaro.Admin.Unity
```
Or if you using Ninject install:
```
Install-Package Ilaro.Admin.Ninject
```
Or you can just install:
```
Install-Package Ilaro.Admin
```
And register by yourself all needed stuff.

##TODO
I don't plan milestones, so I will do things in random order. 
Maybe some of them I'll skip, and I'll probably back to them after release v1.

I moved [TODO](https://github.com/rgonek/Ilaro.Admin/wiki/TODO) to wiki pages, because here, it is doing a mess in commits, and I rather would simple list than an issues pages.

##Requirements:
- POCO classes (or pseudo POCO)
- ASP MVC 4

##Initialisation:

In global.asax you must do three things:

1. Register routes

   ```C#
   // prefix is optional, by default = IlaroAdmin
   AdminInitialise.RegisterRoutes(RouteTable.Routes, prefix: "Admin");
   AdminInitialise.RegisterResourceRoutes(RouteTable.Routes);
   ```
   It should be put before register default routes because you lose a friendly urls
1. Add entities

   ```C#
   Entity<Customer>.Add();
   Entity<Product>.Add();
   ```
   Add method create a Entity object with all info from attributes.
   In future I want add fluent configuration of entity so, there will be no need to configure entity with attributes.
2. Specify access to Ilaro.Admin

   ```C#
   Admin.Authorize = new AuthorizeAttribute(){ Roles = "Admin" };
   ```
   If you don't do that everyone with proper link will have access to Ilaro.Admin.
3. Initialise Ilaro.Admin

   ```C#
   Admin.Initialise("NorthwindEntities");
   ```
   This line initialise UnityContainer, and bind foreign entity and tries set primary key for each entity who has not defeined it. If you have only one ConnectionString there is no need to specify it.
4. Register areas

   ```C#
   AreaRegistration.RegisterAllAreas();
   ```
   This step should be added by default during creating asp mvc project.
   Ilaro.Admin is created as area, so it needs registration.
5. Go to wiki pages for more info. [Entity configuration](https://github.com/rgonek/Ilaro.Admin/wiki/Entity-configuration) [Property configuration](https://github.com/rgonek/Ilaro.Admin/wiki/Property-configuration)
   
And after that when you go to ~/IlaroAdmin url (if you don't define other prefix) you should see something like that:
####Dashboard
![Ilaro.Admin dashboard](https://dl.dropboxusercontent.com/u/3659823/IlaroAdmin/dashboard.png)
####Records list
![Ilaro.Admin records list](https://dl.dropboxusercontent.com/u/3659823/IlaroAdmin/entity_details.png)
####Create new record
![Ilaro.Admin create new record](https://dl.dropboxusercontent.com/u/3659823/IlaroAdmin/create_new_record.png)

##What I use
Here I will try write all libraries, and part of code I use in project.
- [**Massive**](https://github.com/robconery/massive) - for db access. I Removed lots methods I left only read methods, thanks that I have much better control on created commands, and in future I want to completely removed massive.
- [**ImageResizer**](http://imageresizing.net/) - for resizing image. There's no much need to use this library, you can easy get rid off it, but I like it :)
- [**RazorGenerator**](http://razorgenerator.codeplex.com/) and [extension](http://visualstudiogallery.msdn.microsoft.com/1f6ec6ff-e89b-4c47-8e79-d2d68df894ec) - for generating source code from views, thanks that you don't have add views into your project, just simply add dll. Of cource if you want you can add views files which overrides compiled views
- [**Twitter bootstrap**](http://getbootstrap.com/)
- [**Chosen**](http://harvesthq.github.io/chosen/) and bootstrap style for it https://gist.github.com/koenpunt/6424137
- [**Bootstrap-DateTimePicker**](https://github.com/Eonasdan/bootstrap-datetimepicker) - for DateTime picker, Date picker and Time picker
- [**Bootstrap-SpinEdit**](https://github.com/scyv/bootstrap-spinedit) - for numeric editor
- [**Bootstrap-Markdown**](http://toopay.github.io/bootstrap-markdown/) - for markdown editor
- [**Marked**](https://github.com/chjj/marked) - for parsing markdown
- [**Summernote**](https://github.com/HackerWins/summernote) - for html wysiwyg editor
- [**Bootstrap Dual Listbox**](http://www.virtuosoft.eu/code/bootstrap-duallistbox/) - for one to many editor
- [**Bootstrap-file-input**](https://github.com/grevory/bootstrap-file-input) - file input

