Ilaro.Admin
===========

Ilaro.Admin creates for you admin panel using only POCO classes.

Project was inspired by [Django admin site](https://docs.djangoproject.com/en/dev/ref/contrib/admin/).

Please keep in mind this is steel a alpha version.

##Requirements:
- POCO classes (or pseudo POCO)
- ASP MVC 4

##Initialisation:

In global.asax you must do three things:

1. Register routes

   ```C#
   //                                                prefix is optional, by default = IlaroAdmin
   AdminInitialise.RegisterRoutes(RouteTable.Routes, "Admin");
   AdminInitialise.RegisterResourceRoutes(RouteTable.Routes);
   ```
   It should be before register default routes because you lose a friendly urls
2.  Add entities

   ```C#
   AdminInitialise.AddEntity<Customer>();
   AdminInitialise.AddEntity<Product>();
   ```
   AddEntity method create a EntityViewModel object with all info from attributes.
   In future I want add here a fluent configuration of entity so, there is no need to configure entity with attributes.
3.  Initialise Ilaro.Admin

   ```C#
   AdminInitialise.Initialise("NorthwindEntities");
   ```
   This line initialise UnityContainer, and bind foreign entity and try set primary key for each entity who has not defeined it. If you have only one ConnectionString there is no need to specify it.
   
And after that when you go to ~/IlaroAdmin url (if you don't define a other prefix) and you should view something like that:
####Dashboard
![Ilaro.Admin dashboard](https://dl.dropboxusercontent.com/u/3659823/IlaroAdmin/dashboard.png)
####Records list
![Ilaro.Admin records list](https://dl.dropboxusercontent.com/u/3659823/IlaroAdmin/entity_details.png)
####Create new record
![Ilaro.Admin create new record](https://dl.dropboxusercontent.com/u/3659823/IlaroAdmin/create_new_record.png)

##What I use
Here I try write all libraries, and part of code I use in project.
- [**Massive**](https://github.com/robconery/massive) - for db access. I'm added few lines commented like that: // Illaro.Admin, so its easy to find
- [**ImageResizer**](http://imageresizing.net/) - for resizing image. There's no much need to use this library, you can easy get rid off it, but I like it :)
- [**Unity**](http://msdn.microsoft.com/en-us/library/ff647202.aspx) - for resolving stuff :), for now I don't resolve too much things
- [**RazorGenerator**](http://razorgenerator.codeplex.com/) and [extension](http://visualstudiogallery.msdn.microsoft.com/1f6ec6ff-e89b-4c47-8e79-d2d68df894ec) - for generating source code from views, thanks that you don't have add views into your project, just simply add dll. Of cource if you want you can add views files which overrides compiled views
- Pager - I'm start using https://github.com/troygoode/PagedList (I used earlier version, not including dll but copy a code), but I'm use it only for generating pager, so later I'm move generating pager to view and use some other pager algorithm (I found it in stackoverflow, unfortunately I cannot find it now)
- [**Twitter bootstrap**](http://getbootstrap.com/) (and some plugins for it)

##Knowing issue
-  Validation - for validate entity I'm using data annotations attributes (it probably works with custom ValidationAttribute), and client side validation look nice, but problem starts with server side validation.
  
   ```C#
[Required]
[StringLength(20)]
[Compare("Other property")]
public string ProductName { get; set; }
   ```
   In this example Required and StringLength works well but there is problem with Compare (in client side works well).
- Editing entity not work
- Cascade deleting not work (and there is no configuration for it if you want or not cascade delete)
- Foreign entities not work
- Unauthorized access - for now everyone who have proper url can use Ilaro.Admin
- Action history - not implemented
