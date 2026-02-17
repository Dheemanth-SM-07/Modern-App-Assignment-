import React from 'react';
import ProductList from './components/ProductList';
import 'bootstrap/dist/css/bootstrap.min.css';
import './App.css';

function App() {
  return (
    <div className="App">
      <nav className="navbar navbar-expand-lg navbar-dark bg-primary mb-4">
        <div className="container-fluid">
          <span className="navbar-brand mb-0 h1">
            🚀 Modern App - React 18 + .NET 10
          </span>
          <span className="navbar-text text-white">
            Bootstrap 5.3.3 | jQuery 3.7.1
          </span>
        </div>
      </nav>
      
      <ProductList />
      
      <footer className="mt-5 py-4 bg-light text-center">
        <p className="mb-0 text-muted">
          © 2026 Modern App | Upgraded to .NET 10 with Latest Libraries
        </p>
      </footer>
    </div>
  );
}

export default App;
