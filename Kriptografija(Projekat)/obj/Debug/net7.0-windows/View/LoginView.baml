   M S B A M L   `   `   `  ����R  NKriptografija(Projekat), Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullK*clr-namespace:Kriptografija_Projekat_.ViewKriptografija_Projekat_.View  5      U/clr-namespace:Kriptografija_Projekat_.ViewModel!Kriptografija_Projekat_.ViewModel  6G   ,    &Kriptografija_Projekat_.View.LoginView   5      W SPresentationCore, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      <local*clr-namespace:Kriptografija_Projekat_.View     5      E	viewmodel/clr-namespace:Kriptografija_Projekat_.ViewModel     6G     � 	Resources  5
       �� Source�� 5   
   $ /Resources/Styles.xaml?�6   %    .��5      � 
������� � RowDefinitions 5   
   �� 5      �� 5      $
��auto �6   �� 5      $
��auto �6   �� 5      $��* �6   .+�5   
   �� $��Login��6   $��25(�5      $��SemiBold"�5      $��Center=�5      $��Center=�5      $��0��5      �� 5   
   $��1��6      UsernameTextBox#��[  5      $��200,20,200,0q�5      ��5      �� *
Username+5      �� 5    
   $��2��6     PasswordTextBox#��[ 5"      $��200,20,200,0q�5#      ��5!      �� *
Password+5#      � 5$   
   $��3��6    5%      �� 5&      $
��auto �6    �� 5'      $��* �6    .+�5)      �� $��15(�5*      $��0,20,0,0q�5+      $��Center=�5,      $��Light"�5-      
���\\����5)      �� *ErrorMessage+5.      �� 5/      $��1��6   .+�50      �� $��Login��6     LoginButtonStyle#��[ 52      $	��Top=�54      $��0,50,0,0q�55      ��51      �� *LoginCommand+55      �� 56      $
��Back��6     RegisterButtonStyle#��[ 57      $��60��58      $��0,10,0,0q�59      ��5:      �� *NavigateBackCommand+5;      5<      5=      5>      