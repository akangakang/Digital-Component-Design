   M S B A M L   `   `   `  ����E  ALogoScript, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:LogoScriptIDELogoScriptIDE  5   	        LogoScriptIDE.EditorWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   ` \ICSharpCode.AvalonEdit, Version=4.1.0.7916, Culture=neutral, PublicKeyToken=9cc39be672370310?
avalonEdit.http://icsharpcode.net/sharpdevelop/avalonedit  5   	   )myspaceclr-namespace:LogoScriptIDE  5   	   -   5        $	��500��5   	   $	��300��6     � ShowInTaskbar  .   � WindowStyle$ 
ToolWindow=�6h    J� CommandBindings 5      )  #System.Windows.Input.CommandBinding  5   
   -   5           Command$&  myspace:EditorWindow.CommandOpen��5      6D     5	   
   -   5        $&  myspace:EditorWindow.CommandSave��5	      6D     5
   
   -   5        $% myspace:EditorWindow.CommandNew��5
      6C    J� InputBindings 5      �� 5   
    �� Command$&  myspace:EditorWindow.CommandOpen��6   
 �� Key$ O��6@    �� 	Modifiers$
 Ctrlf�6H   �� 5   
   $&  myspace:EditorWindow.CommandSave��6   $ S��6@   $
 Ctrlf�6H   �� 5   
   $% myspace:EditorWindow.CommandNew��6   $ N��6?   $
 Ctrlf�6G   .��5      � .+�5   
   '  !ICSharpCode.AvalonEdit.TextEditor  -   5        $��	ui_editor��5      $��Consolas)�5      $
��11pt(�5         ShowLineNumbers . 	   Text.	 5   H   5      5      5      