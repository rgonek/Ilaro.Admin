Ilaro.Admin
===========

Ilaro.Admin creates for you admin panel using only POCO classes.

[Demo](http://admin.ilaro.net/) - using Northwind DB (with small modifications, removed multiple primary keys)

Project was inspired by [Django admin site](https://docs.djangoproject.com/en/dev/ref/contrib/admin/).

Please keep in mind this is still a alpha version.

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
- [ ] Entities changes
   - [ ] Write sql command text in change description as option
   - [ ] Changes for a specific record
- [ ] Upload files
   - [ ] Handle file upload on create and edit
      - [ ] Delete old files on edit
   - [ ] Delete files on delete
   - [ ] File names
      - [ ] Predefined format for property
      - [ ] User input
- [ ] Composite keys
- [ ] Entity validation
- [ ] Code is almost untested, but should be
- [ ] Disable editing primary key
- [ ] ?Maybe we can get some interesting infos from db schema?
- [ ] Refactor
   - [ ] Move entity.ClearPropertiesValues() to ActionFilterAttribute
   - [ ] Move logic from EntityController to EntityService
   - [ ] Completely remove Massive (there are only read methods left)
   - [ ] IEntityFilters should generate sql with parameters

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
   It should be before register default routes because you lose a friendly urls
2. Add entities

   ```C#
   AdminInitialise.AddEntity<Customer>();
   AdminInitialise.AddEntity<Product>();
   ```
   AddEntity method create a EntityViewModel object with all info from attributes.
   In future I want add here a fluent configuration of entity so, there is no need to configure entity with attributes.
3. Specify access to Ilaro.Admin

   ```C#
   AdminInitialise.Authorize = new System.Web.Mvc.AuthorizeAttribute { Roles = "Admin" };
   ```
   If you don't do that everyone with proper link have access to Ilaro.Admin.
4. Initialise Ilaro.Admin

   ```C#
   AdminInitialise.Initialise("NorthwindEntities");
   ```
   This line initialise UnityContainer, and bind foreign entity and try set primary key for each entity who has not defeined it. If you have only one ConnectionString there is no need to specify it.
5. Go to wiki pages for more info. [Entity configuration](https://github.com/rgonek/Ilaro.Admin/wiki/Entity-configuration) [Property configuration](https://github.com/rgonek/Ilaro.Admin/wiki/Property-configuration)
   
And after that when you go to ~/IlaroAdmin url (if you don't define a other prefix) and you should view something like that:
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
- [**Unity**](http://msdn.microsoft.com/en-us/library/ff647202.aspx) - for resolving stuff :), for now I don't resolve too much things
- [**RazorGenerator**](http://razorgenerator.codeplex.com/) and [extension](http://visualstudiogallery.msdn.microsoft.com/1f6ec6ff-e89b-4c47-8e79-d2d68df894ec) - for generating source code from views, thanks that you don't have add views into your project, just simply add dll. Of cource if you want you can add views files which overrides compiled views
- Pager - I start using https://github.com/troygoode/PagedList (I used earlier version, not including dll but copy a code), but I use it only for generating pager, so later I move generating pager to view and use some other pager algorithm (I found it in stackoverflow, unfortunately I cannot find it now)
- [**Twitter bootstrap**](http://getbootstrap.com/)
- [**Chosen**](http://harvesthq.github.io/chosen/) and bootstrap style for it https://gist.github.com/koenpunt/6424137
- [**Bootstrap-DateTimePicker**](https://github.com/Eonasdan/bootstrap-datetimepicker) - for DateTime picker, Date picker and Time picker
- [**Bootstrap-SpinEdit**](https://github.com/scyv/bootstrap-spinedit) - for numeric editor
- [**Bootstrap-Markdown**](http://toopay.github.io/bootstrap-markdown/) - for markdown editor
- [**Marked**](https://github.com/chjj/marked) - for parsing markdown
- [**Summernote**](https://github.com/HackerWins/summernote) - for html wysiwyg editor
- [**Bootstrap Dual Listbox**](http://www.virtuosoft.eu/code/bootstrap-duallistbox/) - for one to many editor
- [**Bootstrap-file-input**](https://github.com/grevory/bootstrap-file-input) - file input

##Knowing issue
-  Validation - for validate entity I using data annotations attributes (it probably works with custom ValidationAttribute), and client side validation look nice, but problem starts with server side validation.
  
   ```C#
[Required]
[StringLength(20)]
[Compare("Other property")]
public string ProductName { get; set; }
   ```
   In this example Required and StringLength works well but there is problem with Compare (in client side works well).
- Table with multiple primary keys not work
