import React from 'react';
import { useState } from 'react'
import { AppBar, Toolbar, Typography, Button, Container } from '@mui/material';
import { BrowserRouter as Router, Routes, Route, Link } from "react-router-dom";
import { AllCommunityModule, ModuleRegistry } from 'ag-grid-community'; 
import './App.css'
import ListPage from './pages/ListPage';
import FormPage from './pages/FormPage';

function App() {
  // Register all Community features
  ModuleRegistry.registerModules([AllCommunityModule]);

  return (
      <Router>
      <AppBar position="static">
        <Container maxWidth="x1">
        <Toolbar disableGutters>
          <Typography variant="h6" sx={{ flexGrow: 1 }}>Cafe Employee App</Typography>
          <Button color="inherit" component={Link} to="/cafes">
            Cafes
          </Button>
          <Button color="inherit" component={Link} to="/employees">
            Employees
          </Button>
        </Toolbar>
        </Container>
      </AppBar>

      <Routes>
          <Route path="/cafes" index element={<ListPage key="cafes" />} />
          <Route path="/employees" element={<ListPage key="employees" />} />

          <Route path="/cafes/:id" element={<FormPage title="Edit Cafe" populate key="editCafe" fields={['name','location','description']} entity="cafe"/>} />
          <Route path="/employees/:id" element={<FormPage title="Edit Employee" populate key="editEmployee"/>} />

          <Route path="/cafe" element={<FormPage title="Add Cafe" key="addCafe" />} />
          <Route path="/employee" element={<FormPage title="Add Employee" key="addEmployee"/>} />
      </Routes>

    </Router>
    
    
    

    
  )
}

export default App;
