   M S B A M L   `   `   `  ���� G  CMediaTracker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:MediaTrackerMediaTracker  ?$clr-namespace:MediaTracker.MyClassesMediaTracker.MyClasses      MediaTracker.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     ,localclr-namespace:MediaTracker     :	myClasses$clr-namespace:MediaTracker.MyClasses     -     � Icon$3  -pack://application:,,,/assets/images/icon.ico�� � Title$ Media Tracker V2��$	��290��$	��600�� � 	Resources    infoText �� 
TargetType �� Property �� Value  	infoInput  
ControlBtn  	ToggleBtn��%�  &	        &	 �     &	 6    &	 �    �� " ��.���  � ��$ 2q���  �@��Padding$ 2q���  �@��TextAlignment$ Center=���  � ��$ 0����  � ��$ 0���� " ��.���  � ��$ 2 2 6 2q���  �@��
IsReadOnly$
 True����  � �� �#eee��  � ��$ 1����  � ��$ 1����  � ��$	 350���� " ��.���  � ��$ 2q���  � ��$ 2q���  � ��$ 52���� " d�.���  � ��$ 5q���  � ��$	 4 0q�.����  �� Padding$ 2q�
�������._��  � ColumnDefinitions �� -    ��Name$ treeCol��$��* ��� $
��auto ��� -   $ 
trackerCol��$��* �.+�� $��0��	 � RowDefinitions	 �� $
��auto ��� $
��auto ��� $��* �.+�� $��0�� �� $
��auto ��� $��* �.+��� -   $
��Root��$��2q�$��0���� -   
 ��Name$
 rootTextBox��$	��2 0q�$	��2 0q�$
��Root�����#eee$��1�� �� 
IsReadOnly$
 True��� $��2q�$��1�� �� $
��auto ��� $��* ��� $
��auto ��� $
��auto �.+��� $��Search: ��$��0���� -   $
 searchTextBox��
�������$��1���� -    ��Name$ clearSearchBoxBtn��$��X��$	��4 1q�$��2 0 2 0q� � ToolTip$ Refresh Content��$y�Hidden=�
������ $��2���� -   $ 
refreshBtn��$	��⟳��$	��4 1q�$��2 0 2 0q�$ Refresh Content��$��2��R� -	    R�Name$ TreeView��$��2������   �� Orientation �� RelativeSource �� Mode �� AncestorType �� Path3  -MediaTracker.MyClasses.HeaderToImageConverter   Instance �� 	Converter �� Padding��%�   'Q�       �� " Q�.���  � �� �� .R��� $ 
Horizontal=�.+��� $��15������  �� $ FindAncestor=�" Q�$	 Tag�# Z �� $	 5 0q�$��Center=����� �� -
    ��Name$ GridSplitter��$��2q�$��1��$��Center=�$��Stretch=� �� ResizeBehavior$ PreviousAndNext=�$��3��
�����w��� -   $��mainUI��$��2��
�������$ Vertical=�.+�9� -    9�Name$ settingsExpander��$��Settings��.���� 
�����w�$	��1.5q�$��3q�
�������._��� $��5q�.+��� -    ��Name$ autoAdvanceSetting��$��Auto Advance On Open File��$��2q��� $ 
Horizontal=�.+��� $��5 2 5 0q�On Opening File:�� -    ��Name$ OnOpenComboBox��$	��180��.@���  �� 
IsSelected$
 True��$��
Do nothing��
 � Tag$ 0���� $��Minimize window to tray��$ 1���� $��Close application��$ 2��� $��2��$��5q� �� $��* ��� $��2* �.+��� $��Selected Folder: ��#��[  $��0���� -   $
 selectedFolderText��#��[ $��1��� $��2��$��5q� �� $��* ��� $��2* �.+��� $��File's Folder: ��#��[  �� -   $
 selectedFileFolderText��#��[ � $��2��$��5q� �� $��* ��� $��2* �.+��� $��Selected File: ��#��[  �� -   $
 selectedFileText��#��[ �� $��Folder Controls�� �� TextAlignment$ Center=��� $ 
Horizontal=�$��Center=�.+��� -   $ PreviousFolderBtn��$��Previous��#��[ �� -   $ OpenFolderBtn��$
��Open��#��[ �� -   $ NextFolderBtn��$
��Next��#��[ �� $��0 5 0 0q�$��File Controls��$ Center=��� $ 
Horizontal=�$��Center=�.+��� -   $ Previous5FileBtn��$��<<��#��[ �� -   $ PreviousFileBtn��$��Previous��#��[ �� -   $ OpenFileBtn��$
��Open��#��[ �� -   $ NextFileBtn��$
��Next��#��[ �� -   $ Next5FileBtn��$��>>��#��[ 9� -   $ randomExpander��$��Random File��$��0 5 0 0q�.���� .+��� -   $
 
RandomText��#��[ $
��10 4q�$��2���  �� $��* ��� $��* ��� $��* ��� $��* �.+��� -   #��[ $��Generate��$��65��$��1���� -   $ OpenRandomBtn��#��[ $
��Open��$��2��