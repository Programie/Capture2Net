# What is it?

With Capture2Net you are able to create screenshots with shortcuts and push them to a PHP enabled webserver.


## Components

Capture2Net is splitted into multiple components.

### Client

The client is the application running on your computer which creates the screenshots and sends them via HTTP or HTTPS to the webserver.

Currently the only available client is written in .NET Framework 4.5 and therefore requires Windows Vista or higher.


### Webinterface

The webinterface contains the following parts:

* Configuration interface (Allows to change configuration options like picture format, shortcuts, etc.)
* Screenshot browser (Provides access to already uploaded screenshots)
* Backend of the client (Handles uploading of screenshots and client configuration)

The frontend is written in the JavaScript framework qooxdoo.

The backend is written in PHP.
