   M S B A M L   `   `   `  ����  
LogoScript    LogoScriptIDE.StackWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$  	Registers��5   	   $	��210��6   $	��500��6(    � Left$	 100��64   
 � Top$ 50��6?    � ShowInTaskbar .   � WindowStyle$ 
ToolWindow=�6w   .��5      � .+�5   
   &   System.Windows.Controls.DataGrid  -   5        $��ui_grid��5      $
��Auto��5      $��Stretch=�5      $��Stretch=�5	      $
��Auto��5
         
IsReadOnly .     CanUserResizeRows .  $��Stretch=�5      $��Consolas)�5         HeadersVisibility$ Column=�5         CanUserSortColumns .  	   AutoGenerateColumns	 .  
   CanUserReorderColumns
 .  $   HorizontalScrollBarVisibility$
 Auto=�5   M   "   VerticalScrollBarVisibility$ Visible=�6r      Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      -   5        ui_depthColumn��,  &System.Windows.Controls.DataGridColumn   Width5  /System.Windows.Controls.DataGridLengthConverter$
 Auto 5   =      CanUserResize .     Header$	 Reg��6a   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6n   �� *
Register+6�     5      -   5        ui_lineColumn��$
 Auto 5   <    .  $	 Hex��6_    6l   �� *Hex+6�     5      -   5        ui_functionColumn��$
 Auto 5   @    .  $	 Dec��6c    6p   �� *Dec+6�   5!      5"      5#      