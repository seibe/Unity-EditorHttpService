# Unity EditorHttpService

Available on Unity 5.6.1f1

* GET `http://localhost:8889`  
return `Application.unityVersion`
* GET `http://localhost:8889/HasError`  
return `true` or `false`
* GET `http://localhost:8889/Compile`  
run `InternalEditorUtility.RequestScriptReload()`

## LICENSE
Copyright (c) 2017 Seibe TAKAHASHI.

This software is released under the MIT License, see [LICENSE](LICENSE).
