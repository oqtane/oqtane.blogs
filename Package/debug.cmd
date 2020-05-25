XCOPY "..\Client\bin\Debug\netstandard2.1\Oqtane.Blogs.Client.Oqtane.dll" "..\..\oqtane.framework\Oqtane.Server\bin\Debug\netcoreapp3.1\" /Y
XCOPY "..\Client\bin\Debug\netstandard2.1\Oqtane.Blogs.Client.Oqtane.pdb" "..\..\oqtane.framework\Oqtane.Server\bin\Debug\netcoreapp3.1\" /Y
XCOPY "..\Server\bin\Debug\netcoreapp3.1\Oqtane.Blogs.Server.Oqtane.dll" "..\..\oqtane.framework\Oqtane.Server\bin\Debug\netcoreapp3.1\" /Y
XCOPY "..\Server\bin\Debug\netcoreapp3.1\Oqtane.Blogs.Server.Oqtane.pdb" "..\..\oqtane.framework\Oqtane.Server\bin\Debug\netcoreapp3.1\" /Y
XCOPY "..\Shared\bin\Debug\netstandard2.1\Oqtane.Blogs.Shared.Oqtane.dll" "..\..\oqtane.framework\Oqtane.Server\bin\Debug\netcoreapp3.1\" /Y
XCOPY "..\Shared\bin\Debug\netstandard2.1\Oqtane.Blogs.Shared.Oqtane.pdb" "..\..\oqtane.framework\Oqtane.Server\bin\Debug\netcoreapp3.1\" /Y
XCOPY "..\Server\wwwroot\Modules\Oqtane.Blogs\*" "..\..\oqtane.framework\Oqtane.Server\wwwroot\Modules\Oqtane.Blogs\" /Y /S /I
