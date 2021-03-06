# Sitecore Imaging toolkit
Sitecore has native support for resizing images. This library adds support for
image cropping, defining jpeg output quality and produces **optimized progressive
jpeg** images when the source file is a jpg.

## Building
Build the solution and put the resulting DLL's in your Sitecore solution and put
the ImageService.config file in your App_Config/Include folder. You may want to
reference another version of Sitecore Kernel to suit your Sitecore solution. The
package probably works with most Sitecore versions since 6.x, though URL hashing
applies only to later versions of Sitecore.

## Usage
The library can be used in many ways. One way is using the ImageRendering class
to build proper image URLs, such as this:

~~~~
@{
  // Assuming a MediaItem exists
  var imageRendering = new ImageRendering(mediaItem);
}
<picture>
  <source srcset="@imageRendering.WithAutoCrop(400,300).WithQuality(75).GetUri()" ... />
  ...
</picture>
~~~~

If you have a typed model, you can provide the ImageRendering object as an 
extension method to the image field type of the model.

More info about this module can be found here:
[http://mikael.com/2016/09/optimized-progre…ages-in-sitecore/](http://mikael.com/2016/09/optimized-progre…ages-in-sitecore/)

##Credits
The idea and some of the code in this module is based on two blog posts
by Anders Laub [@AndersLaub](https://twitter.com/AndersLaub):
[Setting JPEG compression level](https://laubplusco.net/make-sitecore-deliver-images-fits-screen/)
and [Cropping images](https://laubplusco.net/make-sitecore-deliver-images-fits-screen-part-2/)
