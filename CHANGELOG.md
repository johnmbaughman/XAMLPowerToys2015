# Roadmap

- [ ] Suggest a feature.

Features that have a checkmark are complete and available for
download in the
[CI build](http://vsixgallery.com/extension/d01c1624-8838-4097-bf77-f52d73fc7a1f/).

# Changelog

These are the changes to each version that has been released on the official Visual Studio extension gallery.

## 1.5.32

- [x] All .ToLower() calls converted to .ToLower(CultureInfo.InvariantCulture).

## 1.5.28

- [x] Enabled support for .NET Standard 2.0 Xamarin.Forms projects.


## 1.5.27

- [x] Corrected generated code when field was from a deeply nested class structure.  Example:  Order.Customer.FirstName or Order.Customer.Address.StreetName, these are all now generated correctly.


## 1.5.26

- [x] Added CultureInfo.InvariantCulture argument to ToLower() on file names.


## 1.5.25

- [x] Excluded Xamarin types from reflection.  One customer's system kept throwing on these.


## 1.5.1

- [x] Added customer requested feature to list ViewModel's where the case insensitive class name ends with "viewmodel" or "pagemodel".

## 1.5

- [x] Updated for Visual Studio 2017.
- [x] Fixed xaml serialization to handle special characters entered in label text boxes

## 1.3.21

**2016-09-19**

- [x] If not ItemsSource is selected for ComboBox's or the BindablePicker, the code generator will still bind to SelectedItem.


## 1.3.20

**2016-09-18**

- [x] Enabled Silverlight 5


## 1.2.18

**2016-09-17**

- [x] Enabled reflecting and including internal types in select class list.


## 1.2.17

**2016-07-17**

- [x] Corrected Date Picker Minimum and Maximum binding to view model properties to OneWay binding instead of two way.


## 1.2.16

**2016-07-17**

- [x] On select view model window, set Next button as IsDefault, and the Cancel button as IsCancel


## 1.2.15

**2016-07-17**

- [x] Added ability to render an image instead of a label from the Advanced Options.


## 1.2.10

**2016-07-16**

- [x] Cut support dragging unbound controls to the design surface.  Feature worked in debug builds, but always failed in VSIX builds.  Spent 4 hours trying to fix it, no joy.


## 1.2.9

**2016-07-16**

- [x] Added support for Visual Studio Community Edition


## 1.1.7

**2016-06-25**

- [x] Added support for Windows Universal applications
- [x] Added Content property to WPF CheckBox control.
- [x] For WPF and UWP, no longer code generating extra form wrapping Grid when only one column is rendered.


## 1.0.5

**2016-06-19**

- [x] Changed Slider control XAML Serialization to account for a bug in Xamarin Forms, you must have the Maximum value before Minimum value in your XAML.

## 1.0

**2016-06-18**

- [x] Initial release
- [x] Rapid Data Form Creation for Xamarin Forms and WPF
