   M S B A M L   `   `   `  ����E  ALogoScript, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:LogoScriptIDELogoScriptIDE  5   	       LogoScriptIDE.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   /myspaceclr-namespace:LogoScriptIDE     5   	   -   5          � Title$  
Mips Mania��5   	    � SizeToContent$ Height=�5   	    � 
ResizeMode$ CanMinimize=�5   	   $	��500��5   	    J� CommandBindings 5      )  #System.Windows.Input.CommandBinding  5   
   -   5           Command$$ myspace:MainWindow.CommandOpen��5      6B     5   
   -   5        $$ myspace:MainWindow.CommandSave��5      6B     5   
   -   5        $# myspace:MainWindow.CommandNew��5      6A     5   
   -   5        $$ myspace:MainWindow.CommandUndo��5      6B     5   
   -   5        $$ myspace:MainWindow.CommandRedo��5      6B    J� InputBindings 5      �� 5   
    �� Command$$ myspace:MainWindow.CommandOpen��6   
 �� Key$ O��6>    �� 	Modifiers$
 Ctrlf�6F   �� 5   
   $$ myspace:MainWindow.CommandSave��6   $ S��6>   $
 Ctrlf�6F   �� 5   
   $# myspace:MainWindow.CommandNew��6   $ N��6=   $
 Ctrlf�6E   �� 5   
   $$ myspace:MainWindow.CommandUndo��6   $ Z��6>   $
 Ctrlf�6F   �� 5   
   $$ myspace:MainWindow.CommandRedo��6   $ Y��6>   $
 Ctrlf�6F   .��5   
   ]� $
��Left=�6   $	��Top=�6/   .+�5   
   o� $	��Top=�6   $	��500��6$   .@�5      m� -   5        $��
ui_mnuFile��5   &   $��_File��6   .@�5      m� -   5        $��	ui_mnuNew��5   )   $
��_New��6   	 m� Command$#	 myspace:MainWindow.CommandNew��6:   
 m� InputGestureText$
 Ctrl+N��6b   �� 5       m� 5!      -	   5        $��
ui_mnuOpen��5!   *   $��_Open��6   $$	 myspace:MainWindow.CommandOpen��6<   $
 Ctrl+O��6e   m� 5"      -
   5        $��
ui_mnuSave��5"   )   $
��Save��6   $$	 myspace:MainWindow.CommandSave��6;   $
 Ctrl+S��6d   m� 5#      -   5        $��ui_mnuSaveAs��5#   -   $��_Save As��6   6A   �� 5$      m� 5%      -   5        $��ui_mnuOptions��5%   -   $��_Options��6   6-   �� 5&      m� 5'      -   5        $��
ui_mnuExit��5'   *   $��_Exit��6   6<   5(      m� 5)      -   5        $��ui_mnuEditor��5)   (   $��_Editor��6   .@�5*      m� -   5        $��
ui_mnuUndo��5*   *   $��_Undo��6   $
 Ctrl+Z��6R   m� 5+      -   5        $��
ui_mnuRedo��5+   *   $��_Redo��6   $
 Ctrl+Y��6R   �� 5,      m� 5-      -   5        $��	ui_mnuCut��5-   )   $
��_Cut��6   $
 Ctrl+X��6O   m� 5.      -   5        $��
ui_mnuCopy��5.   *   $��_Copy��6   $
 Ctrl+C��6R   m� 5/      -   5        $��ui_mnuPaste��5/   +   $��_Paste��6   $
 Ctrl+V��6U   �� 50      m� 51      -   5        $��ui_mnuCheckSyntax��51   2   $��_Check Syntax��6   6K   52      m� 53      -   5        $��
ui_mnuView��53   &   $��_View��6   .@�54      m� -   5        $��ui_mnuViewEditor��54   ,   $��_Editor��6    m� 	IsChecked . 6`   m� 56      -   5        $��ui_mnuViewWatches��56   ,   $��_Memory��6    .  6b   m� 57      -   5        $��ui_mnuViewConsole��57   -   $��_Console��6    . 6c   m� 58      -   5        $��ui_mnuViewStack��58   /   $��
_Registers��6    .  6a   59      m� 5:      -   5        $��ui_mnuDebug��5:   '   $��_Debug��6   .@�5;      m� -   5        $��	ui_mnuRun��5;   )   $
��_Run��6   6:   m� 5<      -   5        $��ui_mnuPause��5<   +   $��_Pause��6   6>   m� 5=      -   5        $��
ui_mnuStop��5=   *   $��_Stop��6   6<   �� 5>      m� 5?      -   5        $��ui_mnuStepin��5?   -   $��Step _In��6   6A   m� 5A      -   5        $��ui_mnuStepover��5A   /   $��
Step _Over��6   6E   5B      m� 5C      -    5        $��
ui_mnuHelp��5C   &   $��_Help��6   .@�5D      m� -!   5        $��ui_mnuAbout��5D   +   $��_About��6   6>   5E      5G      c� 5H   
   $	��Top=�6   
�������$	��480��6>    � DataContext 6J   �� .@�5I      �� -"   5        $��
ui_toolNew��5I   D   $��30��6-   $��30��68   $#��myspace:MainWindow.CommandNew��6V    6   �� .��5J      �� $.��(pack://application:,,,/Component/New.png��6   5K      �� 5L      -#   5        $��ui_toolOpen��5L   D   $��30��6-   $��30��68   $$��myspace:MainWindow.CommandOpen��6W    6   �� .��5M      �� $/��)pack://application:,,,/Component/Open.png��6   5N      �� 5O      -$   5        $��ui_toolSave��5O   D   $��30��6-   $��30��68   $$��myspace:MainWindow.CommandSave��6W    6   �� .��5P      �� $/��)pack://application:,,,/Component/Save.png��6   5Q      �� 5R      �� 5S      -%   5        $��ui_toolUndo��5S   D   $��30��6-   $��30��68   $$��myspace:MainWindow.CommandUndo��6W    6   �� .��5T      �� $/��)pack://application:,,,/Component/Undo.ico��6   5U      �� 5V      -&   5        $��ui_toolRedo��5V   D   $��30��6-   $��30��68   $$��myspace:MainWindow.CommandRedo��6W    6   �� .��5W      �� $/��)pack://application:,,,/Component/Redo.ico��6   5X      �� 5Y      �� 5Z      -'   5        $��
ui_toolRun��5Z   D   $��30��6-   $��30��68    6   �� .��5[      �� $.��(pack://application:,,,/Component/Run.png��6   5\      �� 5]      -(   5        $��ui_toolPause��5]   D   $��30��6-   $��30��68    6   �� .��5^      �� $0��*pack://application:,,,/Component/Pause.png��6   5_      �� 5`      -)   5        $��ui_toolStop��5`   D   $��30��6-   $��30��68    6   �� .��5a      �� $/��)pack://application:,,,/Component/Stop.png��6   5b      �� 5c      -*   5        $��ui_toolStepIn��5c   D   $��30��6-   $��30��68    6   �� .��5d      �� $1��+pack://application:,,,/Component/StepIn.png��6   5e      �� 5f      -+   5        $��ui_toolStepOver��5f   D   $��30��6-   $��30��68    6   �� .��5g      �� $3��-pack://application:,,,/Component/StepOver.png��6   5h      �� 5l      �� 5m      -,   5        $��ui_toolExit��5m   D   $��30��6-   $��30��68    6   �� .��5n      �� $0��*pack://application:,,,/Component/Close.png��6   5o      5p      5q      5r      