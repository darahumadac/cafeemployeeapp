import React from 'react';
import { useState } from 'react'
import { AppBar, Toolbar, Typography, Button, Container } from '@mui/material';
import { BrowserRouter as Router, Routes, Route, Link, Navigate } from "react-router-dom";
import { AllCommunityModule, ModuleRegistry } from 'ag-grid-community'; 
import './App.css'
import ListPage from './pages/ListPage';
import FormPage from './pages/FormPage';
import EmployeeFormPage from './pages/EmployeeFormPage';

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
          <Route path="/" element={<Navigate to="/cafes" replace />} />
          <Route path="/cafes" index element={<ListPage key="cafes" />} />
          <Route path="/employees" element={<ListPage key="employees"/>} />

          <Route path="/cafes/:id" element={<FormPage title="Edit Cafe" populate key="editCafe" entity="cafe"/>} />
          <Route path="/employees/:id" element={<EmployeeFormPage title="Edit Employee" populate key="editEmployee" entity="employee"/>} />

          <Route path="/cafe" element={<FormPage title="Add Cafe" key="addCafe" />} />
          <Route path="/employee" element={<EmployeeFormPage title="Add Employee" key="addEmployee" populate={false}/>} />
      </Routes>

    </Router>
    
    
    

    
  )
}

export default App;
