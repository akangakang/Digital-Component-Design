   M S B A M L   `   `   `  ����   
LogoScript"    LogoScriptIDE.WatchesWindow1   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -     � Title$  Watch��$	��250��$	��450�� � WindowStyle$ 
ToolWindow=� � ShowInTaskbar .  .��� .+�&   System.Windows.Controls.DataGrid  -   $��ui_watchesgrid��
������    AutoGenerateColumns .  $��Stretch=�$��Stretch=�   CanUserReorderColumns .     
IsReadOnly .  $��	0,1,188,0q�   Columns 0  *System.Windows.Controls.DataGridTextColumn  -   ui_nameColumn��,  &System.Windows.Controls.DataGridColumn   Width5  /System.Windows.Controls.DataGridLengthConverter$
 Auto    CanUserResize . 	   Header$
	 Name��1  +System.Windows.Controls.DataGridBoundColumn
   Binding
 �� *Name+  -   ui_valueColumn��$
 Auto  . $	 Value��
 �� *Value+  -   ui_remarksColumn��$
 Auto  . $	 Remarks��
 �� *	Remarks+�� -   $��
ui_addLine��$��	Add Watch��$��	261,2,0,3q�$
��Left=�$��78���� -   $��ui_deleteLine��$��Delete Watch��$��0,1,0,3q�$��Right=�$��83��