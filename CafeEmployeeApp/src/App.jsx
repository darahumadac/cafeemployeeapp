import React from 'react';
import { useState } from 'react'
import { AppBar, Toolbar, Typography, Button, Container } from '@mui/material';
import { BrowserRouter as Router, Routes, Route, Link } from "react-router-dom";
import { AllCommunityModule, ModuleRegistry } from 'ag-grid-community'; 
import './App.css'
import CafesPage from './pages/CafesPage';
import EditCafePage from './pages/EditCafePage';
import EditEmployeePage from './pages/EditEmployeePage';
import AddCafePage from './pages/AddCafePage';
import AddEmployeePage from './pages/AddEmployeePage';

function App() {
  // Register all Community features
  ModuleRegistry.registerModules([AllCommunityModule]);

  return (
      <Router>
      <AppBar position="static">
        <Container maxWidth="x1">
        <Toolbar disableGutters>
          <Typography variant="h6" sx={{ flexGrow: 1 }}>Cafe Employee App</Typography>
        </Toolbar>
        </Container>
      </AppBar>

      <Routes>
          <Route path="/" element={<CafesPage />} />
          <Route path="/cafes/add" element={<AddCafePage />} />
          <Route path="/employees/add" element={<AddEmployeePage />} />
          <Route path="/cafes/edit/:id" element={<EditCafePage />} />
          <Route path="/employees/edit/:id" element={<EditEmployeePage />} />
      </Routes>

    </Router>
    
    
    

    
  )
}

export default App;
