// Encapsulate the ToDo SPA in IIFE (immediately-invoked function expression)
// Dependencies: jQuery, Bootstrap 3.x, vue.min.js and vue-router.min.js
//    See <script> tags at bottom of todoSpaUi.html for details (load order, etc.)
(function (exports) {

    // *** Section 1: define reusable local/private utility functions ***

    // Define Todo array filters
    var filterNames = ['all', 'active', 'complete'];
    var filters = {
        all: function (todos) {
            return todos;
        },
        active: function (todos) {
            return todos.filter(function (todo) {
                return !todo.Complete;
            });
        },
        complete: function (todos) {
            return todos.filter(function (todo) {
                return todo.Complete;
            });
        }
    };

    // Create Web API request for todos resource, maps to C# method named "getUserTodos"
    var getUserTodos = function (todoId, callback) {
        var userId = sessionStorage.getItem("userId");
        var path = "api/users/" + userId + "/todos";
        if (todoId) path += "/" + todoId;
        $.getJSON(path, function (data) {
            if (callback) callback(data);
        });
    };

    // Create Web API request for users resource, maps to C# method named "getUserNames"
    var getUserNames = function (callback) {
        var path = "api/users/names";
        $.getJSON(path, function (data) {
            if (callback) callback(data);
        });
    };
    
    // Return stateful object to be used by setComplete method in both 
    //    the TodoList and TodoEdit components
    var toggleCheckboxCss = function (todo) {
        var rtnObj = {
            'btn-default': !todo.Complete,
            'btn-success': todo.Complete,
            'active': todo.Complete
        };
        return rtnObj;
    };

    // jQuery config of big, honking spinner to make every AJAX request painfully obvious!
    // Somewhat of a hack, slammed in, last minute for demo. purposes
    $(document).ajaxStart(function () { $("img#loading-spinner").show(); });
    $(document).ajaxComplete(function () { $("img#loading-spinner").hide(); });


    // *** Section 2: define List, Add, Edit component constructors ***

    // --------------- List component instance ---------------
    //   Using the Vue.extend (rather than the Vue.component because
    //     the ToDo app does not need component tag names; it's a very
    //     simple 1-to-1 between SPA route, (or page), and component.
    //     I.e. there is no nesting of components in the root Vue instance, 
    //        so a more formal and declaraive approach is unnesssary.
    //     Note: 'exports.TodoList = Vue.component( '', {' accomplish the 
    //            same thing.
    //       
    exports.TodoList = Vue.extend({
        template: '#todo-list',
        data: function () {
            return {
                todos: [],
                currentFilterIndex: 0,
                idPrefix: 'todoId_',
                initDetailsHeight: "55px",
                testMode: true
            };
        },
        mounted: function () {
            var self = this;
            getUserTodos(null, function (data) {
                self.todos = data;
            });
        },
        computed: {
            filteredTodos: function () {
                var filterName = filterNames[this.currentFilterIndex];
                var filteredList = filters[filterName](this.todos);
                return filteredList;
            },
            statusMessage: function () {
                var count = this.filteredTodos.length;
                var filterName = filterNames[this.currentFilterIndex];
                filterName = filterName.charAt(0).toUpperCase() + filterName.slice(1);
                var str = filterName + " todos (" + count.toString() + " " + this.pluralize('item', count) + ")";
                return str;
            }
        },
        methods: {
            doRouterLink: function (todo) {
                // Execute a programmatic route push
                //exports.$TodoRouter.push({ name: 'todo-edit', params: { todo_id: todo.RecId } });
                exports.$TodoRouter.push({ path: '/todo/' + todo.RecId + '/edit/' });
            },
            altRow: function (todo, index) {
                return {
                    "odd-color": index % 2 === 0,
                    "even-color": index % 2 !== 0,
                    "todo-complete": todo.Complete
                };
            },
            setCompleteCss: function (todo) {
                return toggleCheckboxCss(todo);
            },
            toggleComplete: function (todo) {
                var self = this;
                var userId = sessionStorage.getItem("userId");
                var toggleComplete = todo.Complete ? false : true;
                $.ajax({
                    url: "api/users/" + userId + "/todos/" + todo.RecId + "/changeofstatus",
                    type: 'POST',
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify(toggleComplete),
                    success: function () {
                        todo.Complete = toggleComplete;
                    },
                    error: function () {
                        alert("Network error, unable to togggle completion status.");
                    }
                });
            },
            cleanText: function (str) {
                // Strip carriage return and/or line feed from text entered via textarea 
                if (str !== null && str.trim().length > 0) {
                    str = str.replace(/(?:\r\n|\r|\n)/g, '<br />');
                }
                return str;
            },
            toggleFilter: function () {
                ++this.currentFilterIndex;
                if (this.currentFilterIndex > 2)
                    this.currentFilterIndex = 0;
            },
            pluralize: function (word, count) {
                return word + (count === 1 ? '' : 's');
            }
        }
    });

    // --------------- Add component instance (CRUD - Create) ---------------
    exports.TodoAdd = Vue.extend({
        template: '#todo-add',
        data: function () {
            return {
                todo: { fkUserId:"" },
                userNames: [],
                testMode: true
            };
        },
        mounted: function () {
            var todoId = this.$route.params.todo_id;
            var self = this;
            getUserNames(function (data) {
                self.userNames = data;
            });
        },
        methods: {
            addTodo: function () {
                var self = this;
                var userId = sessionStorage.getItem("userId");
                $.ajax({
                    url: "api/users/" + userId + "/todos",
                    type: 'POST',
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify(self.todo),
                    success: function () {
                        $TodoRouter.push('/');
                    },
                    error: function () {
                        alert("Network error, unable to add new todo.");

                    }
                });
            }
        }
    });

    // --------------- Edit component instance  (CRUD - Update) ---------------
    exports.TodoEdit = Vue.extend({
        template: '#todo-edit',
        data: function () {
            return {
                todo: {},
                userNames: [],
                testMode: true
            };
        },
        mounted: function () {
            var todoId = this.$route.params.todo_id;
            var self = this;
            getUserNames(function (data) {
                self.userNames = data;
                // Nest ajax todo's inside success callback
                getUserTodos(todoId, function (data) {
                    self.todo = data[0];
                });
            });
        },
        methods: {
            updateTodo: function () {
                var self = this;
                var userId = sessionStorage.getItem("userId");
                $.ajax({
                    url: "api/users/" + userId + "/todos/" + self.todo.RecId,
                    type: 'PUT',
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify(self.todo),
                    success: function () {
                        $TodoRouter.push('/');
                    },
                    error: function () {
                        alert("Network error, unable to update todo.");

                    }
                });
            },
            setCompleteCss: function (todo) {
                return toggleCheckboxCss(todo);
            },
            toggleComplete: function (todo) {
                todo.Complete = todo.Complete ? false : true;
            }
        }
    });


    // *** Section 3: define Routes and root Vue instance ***

    // ---------------------------------------------------------------------
    // Define routes (for vue-router/js, see https://router.vuejs.org/ for doco) 
    // note: named $TodoRouter for easy alphabetical reference in browser Dev tools
    // The 'name' setting (named routes) is optional, but it can allow for more
    //    readable navigation links, via router-link element or programmtic
    //    $TodoRouter.push()
    // 
    // ---------------------------------------------------------------------
    exports.$TodoRouter = new VueRouter({
        routes: [
          { path: '/', component: TodoList },
          { path: '/todo-add', component: TodoAdd, name: 'todo-add'},
          { path: '/todo/:todo_id/edit', component: TodoEdit, name: 'todo-edit' }
        ]
    });


    // Define global navigation guard to verify valid user state prior to resolve
    // see https://router.vuejs.org/en/advanced/navigation-guards.html
    exports.$TodoRouter.beforeResolve(function (to, from, next) {
        var r = sessionStorage.getItem("userId");
        if ( !r || isNaN(parseFloat(r)) || !isFinite(r) )
            alert("Please select a demo user.");
        else next();
    });

    // Finally, define the root vue instance via Vue constructor
    // note: named $TodoApp for easy alphabetical reference in browser Dev tools
    exports.$TodoApp = new Vue({
        el: '#app',
        router: exports.$TodoRouter,
        template: '<router-view></router-view>'
    });

})(this || window);
