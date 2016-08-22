# ZenKeeper
A simple password generator/manager created for personal use.

Built in C# to be used on Windows.

# Configuration
There are a few things you can change to your preference.
- Hotkey can be changed in the `hook.RegisterHotKey` at Line ~72 of Form1.cs
- It is recommended to change the salt at the start of CryptoURL.cs to a custom value.
- It is recommended to change the Environment Variable value set in `Form1_Load` to a custom value. This is used as a secondary salt for encryption/decryption, on top of the Master Password.
