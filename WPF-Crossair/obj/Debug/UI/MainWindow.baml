   M S B A M L   `   `   `  ����  WPF-Crossair    WPF_Crosshair.MainWindow   5   	   'tb http://www.hardcodet.net/taskbar  5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  
MainWindow��5   	    � WindowStyle$
 None=�5      |�.   � ShowInTaskbar .  
������  � ShowActivated .   � AllowsTransparency . ��5      �� *Height+ �� Mode$ TwoWay=���6C   �� *Width+$ TwoWay=� � Topmost 5      �� *	TopMost+ J� Opacity 5      �� *	Opacity+ � Left 6   �� *Left+$ TwoWay=�
	 � Top	 6B   �� *Top+$ TwoWay=�{�5	      �� *	IsEnabled+
 � 	Resources
 5
       ��Name   TrayContext��%�   &	        �� 5      $ TrayContext��6   .@�5      m� $��Toggle��6   m� 5      $��Options��6   �� 5      m� 5      $
��Exit��6   5      5      .��5      �� -   5        $��RetImage��5   
   ��6   �� *
Reticule+68   5      