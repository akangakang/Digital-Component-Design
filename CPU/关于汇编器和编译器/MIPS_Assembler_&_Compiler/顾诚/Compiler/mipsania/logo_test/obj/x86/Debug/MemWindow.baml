   M S B A M L   `   `   `  ����  
LogoScript    LogoScriptIDE.MemWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$  InitialMemory��5   	   $	��210��6   $	��500��6,    � Left$	 100��68   
 � Top$ 50��6C    � ShowInTaskbar .   � WindowStyle$ 
ToolWindow=�6{   .��5      � .+�5   
   &   System.Windows.Controls.DataGrid  -   5        $��ui_grid��5         AutoGenerateColumns .     Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5	      -   5        ui_depthColumn��,  &System.Windows.Controls.DataGridColumn   Width5  /System.Windows.Controls.DataGridLengthConverter$
 Auto 5	   =      CanUserResize .  	   Header$	 Address��6a   1  +System.Windows.Controls.DataGridBoundColumn
   Binding
 6r   �� *	Address+6�     5
      -   5        ui_lineColumn��$
 Auto 5
   <    .  $		 Hex��6_   
 6l   �� *Hex+6�     5      -   5        ui_functionColumn��$
 Auto 5   @    .  $		 Dec��6c   
 6p   �� *Dec+6�   5      5      5      