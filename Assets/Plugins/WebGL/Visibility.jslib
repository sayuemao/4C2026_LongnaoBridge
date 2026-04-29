mergeInto(LibraryManager.library, {
  RegisterVisibilityChange: function (gameObjectNamePtr, methodNamePtr) {
    var go = UTF8ToString(gameObjectNamePtr);
    var method = UTF8ToString(methodNamePtr);

    function send(state) {
      if (typeof SendMessage === 'function') {
        SendMessage(go, method, state);
        return;
      }

      if (typeof window !== 'undefined') {
        if (window.unityInstance && typeof window.unityInstance.SendMessage === 'function') {
          window.unityInstance.SendMessage(go, method, state);
          return;
        }
        if (window.gameInstance && typeof window.gameInstance.SendMessage === 'function') {
          window.gameInstance.SendMessage(go, method, state);
          return;
        }
      }
    }

    function handler() {
      var state = document.hidden ? "hidden" : "visible";
      send(state);
    }

    document.addEventListener('visibilitychange', handler);
    window.addEventListener('blur', function () { send("hidden"); });
    window.addEventListener('focus', function () { send("visible"); });

    // 初始化同步一次
    handler();
  }
});