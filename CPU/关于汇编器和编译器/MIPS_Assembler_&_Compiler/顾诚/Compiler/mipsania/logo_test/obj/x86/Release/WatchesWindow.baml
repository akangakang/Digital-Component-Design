   M S B A M L   `   `   `  ����   
LogoScript!    LogoScriptIDE.WatchesWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -     � Title$  Memory��$	��210��$	��500�� � Left$	 100��
 � Top$ 50�� � ShowInTaskbar .   � WindowStyle$ 
ToolWindow=�.��� .+�&   System.Windows.Controls.DataGrid  -   $��ui_grid��$
��Auto��$��Stretch=�$��Stretch=�$
��Auto��   
IsReadOnly .     CanUserResizeRows .  $��Stretch=�$��Consolas)�   HeadersVisibility$ Column=�   CanUserSortColumns . 	   AutoGenerateColumns	 .  
   CanUserReorderColumns
 .  $   HorizontalScrollBarVisibility$
 Auto=�"   VerticalScrollBarVisibility$ Visible=�   Columns 0  *System.Windows.Controls.DataGridTextColumn  -   ui_depthColumn��,  &System.Windows.Controls.DataGridColumn   Width5  /System.Windows.Controls.DataGridLengthConverter$
 Auto    CanUserResize .     Header$ Address��1  +System.Windows.Controls.DataGridBoundColumn   Binding �� *	Address+  -   ui_lineColumn��$
 Auto  .  $	 Hex�� �� *Hex+  -   ui_functionColumn��$
 Auto  .  $	 Dec�� �� *Dec+